using System;
using System.IO;
using System.Linq;
using System.Text;
using MimeKit;
using NUnit.Framework;
using Frends.IMAP.ReadEmail.Definitions;
using DateTime = System.DateTime;

namespace Frends.IMAP.ReadEmail.Tests;
public class IMAPTests
{
    private string _user = "user";
    private string _password = "password";
    private string _host = "host";
    private string _directory = "c:/_temp";
    // Dummy message for testing attachments without access to server
    private MimeMessage _dummyMessage = new MimeMessage();


    private MimeMessage PrepareDummyMessage()
    {
        var message = new MimeMessage();
        var path = Path.GetTempFileName();

        message.From.Add(new MailboxAddress("Frend1", "frend1@frends.com"));
        message.To.Add(new MailboxAddress("Frend2", "frend2@frends.com"));
        message.Subject = "Frends";
        message.MessageId = "2137";
        var body = new TextPart("plain")
        {
            Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
            sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
            Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris 
            nisi ut aliquip ex ea commodo consequat."
        };

        // Create dummy attachemnt file
        var stream = new MemoryStream(
            Encoding.UTF8.GetBytes("This is some text in the file."));

        // Create an text attachment for the file located at path
        var attachment = new MimePart("text", "txt")
        {
            Content = new MimeContent(stream, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(path)
        };

        // Now create the multipart/mixed container to hold the message text and the
        // image attachment.
        var multipart = new Multipart("mixed");
        multipart.Add(body);
        multipart.Add(attachment);

        // now set the multipart/mixed as the message body
        message.Body = multipart;


        //purge file when it's stored as attachment
        File.Delete(path);

        return message;
    }

    [SetUp]
    public void Setup()
    {
        // Setting for this integration test are taken from Confluence
        var passwordFromEnvironment = Environment.GetEnvironmentVariable("IMAP_PASSWORD");
        _user = "frends.tests@outlook.com";
        _password = passwordFromEnvironment != null ? passwordFromEnvironment : "password";
        _host = "outlook.office365.com";
        _directory = $"./../../../../_temp";
        _dummyMessage = PrepareDummyMessage();
    }

    [Test]
    public void ReadEmailTest()
    {
        var imapSettings = new Settings
        {
            UserName = _user,
            Password = _password,
            Host = _host,
            Port = 993,
            AcceptAllCerts = true,
            UseSSL = true
        };
        var imapOptions = new Options
        {
            DeleteReadEmails = false,
            GetOnlyUnreadEmails = false,
            MarkEmailsAsRead = true,
            MaxEmails = 10,
            SaveAttachments = false,
            CreateDirectoryIfNotFound = true,
            SavedAttachmentsDirectory = "dummy"
        };
        // We expect there to be at least 1 email in the mailbox
        // otherwise test will fail
        var result = IMAP.ReadEmail(imapSettings, imapOptions);
        Assert.Greater(result.Emails.Count, 0);
        Assert.IsTrue(result.Emails[0].Id.Length > 0);
        Assert.IsNotNull(result.Emails[0].Cc);
        Assert.IsNotNull(result.Emails[0].Date);
        Assert.AreNotEqual(result.Emails[0].Date, default(DateTime));
        Assert.IsTrue(result.Emails[0].From.Length > 0);
        Assert.IsTrue(result.Emails[0].To.Length > 0);
        Assert.IsTrue(result.Emails[0].Subject.Length > 0);
        Assert.IsTrue(result.Emails[0].BodyText.Length > 0);
    }

    [Test]
    public void SaveAttachmentsTest()
    {
        Directory.CreateDirectory(_directory);
        var dir = IMAP.SaveMessageAttachments(_directory, false, _dummyMessage).First();
        Assert.IsTrue(File.Exists(dir));
        Directory.Delete(_directory, true);
    }

    [Test]
    public void SaveAttachmentsAutocreateTest()
    {
        var dir = IMAP.SaveMessageAttachments(_directory, true, _dummyMessage).First();
        Assert.IsTrue(File.Exists(dir));
        Directory.Delete(_directory, true);
    }

    [Test]
    public void GenerateFilePathTest()
    {
        var dir = IMAP.GenerateAttachmentFilePath(
            _dummyMessage.Attachments.First(),
            $"{_directory}/{_dummyMessage.MessageId}");
        Assert.IsTrue(dir.Length > 0);
    }
}
