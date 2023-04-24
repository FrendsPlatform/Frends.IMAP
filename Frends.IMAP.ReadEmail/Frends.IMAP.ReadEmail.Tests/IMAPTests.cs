using NUnit.Framework;
using System;

namespace Frends.IMAP.ReadEmail.Tests
{
    public class IMAPTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReadEmailTest()
        {
            // Setting for this integration test are taken from Confluence
            var passwordFromEnvironment = Environment.GetEnvironmentVariable("IMAP_PASSWORD");
            var imapSettings = new IMAPSettings
            {
                UserName = "emwiwi@wp.pl",
                Password = "tajnehaslo2137",
                Host = "imap.wp.pl",
                Port = 993,
                AcceptAllCerts = true,
                UseSSL = true,
                SaveAttachments = true,
                SavedAttachmentsDirectory = "/home/mike/Projects/_temp"
            };
            var imapOptions = new IMAPOptions
            {
                DeleteReadEmails = false,
                GetOnlyUnreadEmails = false,
                MarkEmailsAsRead = true,
                MaxEmails = 10,
                CreateDirectoryIfNotFound = false
            };
            // We expect there to be at least 1 email in the mailbox, otherwise test will fail
            var result = IMAP.ReadEmail(imapSettings, imapOptions);
            Assert.Greater(result.Count, 0);
            Assert.IsTrue(result[0].From.Length > 0);
            Assert.IsTrue(result[0].To.Length > 0);
            Assert.IsTrue(result[0].Subject.Length > 0);
            Assert.IsTrue(result[0].BodyText.Length > 0);
        }
    }
}