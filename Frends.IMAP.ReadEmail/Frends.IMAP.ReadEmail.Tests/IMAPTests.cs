using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Frends.IMAP.ReadEmail.Tests
{
    public class IMAPTests
    {
        //UserName = "frends.tests@outlook.com",
        //Password = passwordFromEnvironment,
        //Host = "outlook.office365.com",
        private string _user = "emwiwi@wp.pl";
        private string _password = "tajnehaslo2137";
        private string _host = "imap.wp.pl";
        private string _directory = "/home/mike/Projects/_temp";

        [SetUp]
        public void Setup()
        {
            // Setting for this integration test are taken from Confluence
            var passwordFromEnvironment = Environment.GetEnvironmentVariable("IMAP_PASSWORD");
            //_user = "frends.tests@outlook.com";
            //_password = passwordFromEnvironment != null ? passwordFromEnvironment : "password";
            //_host = "outlook.office365.com";
            //_directory = $"{System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.FullName}/_temp";
        }

        [Test]
        public void ReadEmailTest()
        {
            var imapSettings = new IMAPSettings
            {
                UserName = _user,
                Password = _password,
                Host = _host,
                Port = 993,
                AcceptAllCerts = true,
                UseSSL = true
            };
            var imapOptions = new IMAPOptions
            {
                DeleteReadEmails = false,
                GetOnlyUnreadEmails = false,
                MarkEmailsAsRead = true,
                MaxEmails = 10,
                SaveAttachments = false,
            };
            // We expect there to be at least 1 email in the mailbox, otherwise test will fail
            var result = IMAP.ReadEmail(imapSettings, imapOptions);
            Assert.Greater(result.Count, 0);
            Assert.IsTrue(result[0].From.Length > 0);
            Assert.IsTrue(result[0].To.Length > 0);
            Assert.IsTrue(result[0].Subject.Length > 0);
            Assert.IsTrue(result[0].BodyText.Length > 0);
        }

        [Test]
        public void SaveAttachmentsTest()
        {
            var imapSettings = new IMAPSettings
            {
                UserName = _user,
                Password = _password,
                Host = _host,
                Port = 993,
                AcceptAllCerts = true,
                UseSSL = true
            };
            var imapOptions = new IMAPOptions
            {
                DeleteReadEmails = false,
                GetOnlyUnreadEmails = false,
                MarkEmailsAsRead = true,
                MaxEmails = 10,
                CreateDirectoryIfNotFound = true,
                SaveAttachments = true,
                //Getting path to task directory
                SavedAttachmentsDirectory = _directory
            };

            var result = IMAP.ReadEmail(imapSettings, imapOptions);
            var exist = Directory.Exists(imapOptions.SavedAttachmentsDirectory);

            // We expect there to be at least 1 email in the mailbox, otherwise test will fail
            Assert.Greater(result.Count, 0);
            Assert.IsTrue(result[0].From.Length > 0);
            Assert.IsTrue(result[0].To.Length > 0);
            Assert.IsTrue(result[0].Subject.Length > 0);
            Assert.IsTrue(result[0].BodyText.Length > 0);

            // We expect there to be at least 1 email with attachment
            foreach (var msg in result)
            {
                if (msg.SavedAttachmentsPaths.Any())
                {
                    foreach (var file in msg.SavedAttachmentsPaths)
                    {
                        exist &= File.Exists(file);
                    }
                }
            }

            Assert.IsTrue(exist);
        }

        [Test]
        public void SaveWithDirectoryCreationTest()
        {
            // Setting for this integration test are taken from Confluence
            var passwordFromEnvironment = Environment.GetEnvironmentVariable("IMAP_PASSWORD");
            var imapSettings = new IMAPSettings
            {
                UserName = _user,
                Password = _password,
                Host = _host,
                AcceptAllCerts = true,
                UseSSL = true
            };
            var imapOptions = new IMAPOptions
            {
                DeleteReadEmails = false,
                GetOnlyUnreadEmails = false,
                MarkEmailsAsRead = true,
                MaxEmails = 10,
                CreateDirectoryIfNotFound = true,
                SaveAttachments = true,
                //Getting path to task directory
                //SavedAttachmentsDirectory = $"{System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}/_temp"
                //SavedAttachmentsDirectory = "/home/mike/Projects/_temp"
            };
        }
    }
}
