using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        _password = passwordFromEnvironment ?? "password";
        _host = "outlook.office365.com";
        _directory = $"./../../../../_temp";
        _dummyMessage = PrepareDummyMessage();
    }

    [Test]
    [Explicit("Requires a live IMAP account that permits password-based LOGIN.")]
    public void ReadEmailTest()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("IMAP_INTEGRATION_TESTS"),
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            Assert.Ignore("Set IMAP_INTEGRATION_TESTS=true to run this integration test.");
        }

        var imapConnection = new Connection
        {
            UserName = _user,
            Password = _password,
            Host = _host,
            Port = 993,
            AcceptAllCerts = true,
            UseSSL = true
        };
        var imapInput = new Input
        {
            GetOnlyUnreadEmails = false,
            MaxEmails = 10,
        };
        var imapOptions = new Options
        {
            DeleteReadEmails = false,
            MarkEmailsAsRead = true,
            SaveAttachments = false,
            CreateDirectoryIfNotFound = true,
            SavedAttachmentsDirectory = "dummy"
        };
        // We expect there to be at least 1 email in the mailbox
        // otherwise test will fail
        var result = IMAP.ReadEmail(imapInput, imapConnection, imapOptions, CancellationToken.None);
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
        var dir = IMAP.SaveMessageAttachments(
            _directory, false, _dummyMessage).First();
        Assert.IsTrue(File.Exists(dir));
        Directory.Delete(_directory, true);
    }

    [Test]
    public void SaveAttachmentsAutocreateTest()
    {
        var dir = IMAP.SaveMessageAttachments(
            _directory, true, _dummyMessage).First();
        Assert.IsTrue(File.Exists(dir));
        Directory.Delete(_directory, true);
    }

    [Test]
    public void GenerateAttachmentFilePathTest()
    {
        var attachment = _dummyMessage.Attachments.First();
        var basePath = $"{_directory}/{_dummyMessage.MessageId}";
        var filePath = IMAP.GenerateAttachmentFilePath(attachment, basePath);
        Assert.AreEqual(
            $"{basePath}/{((MimePart)attachment).FileName}",
            filePath);
    }

    [Test]
    public void GenerateAttachmentFilePath_MessagePart()
    {
        var messagePart = new MessagePart();
        var basePath = $"{_directory}/{_dummyMessage.MessageId}";
        var filePath = IMAP.GenerateAttachmentFilePath(messagePart, basePath);
        Assert.IsTrue(filePath.StartsWith($"{basePath}/attached-message"));
    }

    [Test]
    public void ReadEmail_ReadsMessagesFromServer()
    {
        var raw = "From: a@frends.com\r\nTo: b@frends.com\r\nCc: c@frends.com\r\nSubject: Hello\r\n" +
                  "Message-Id: <1@frends.com>\r\nDate: Mon, 1 Jan 2024 10:00:00 +0000\r\n\r\nBody text\r\n";
        using var server = new FakeImapServer(new List<string> { raw, raw });
        var connection = new Connection
        {
            UserName = "user",
            Password = "password",
            Host = "127.0.0.1",
            Port = server.Port,
            UseSSL = false,
        };
        var input = new Input { MaxEmails = 1, GetOnlyUnreadEmails = true };
        var options = new Options { MarkEmailsAsRead = true, DeleteReadEmails = true };

        var result = IMAP.ReadEmail(input, connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Emails.Count);
        Assert.AreEqual("Hello", result.Emails[0].Subject);
        Assert.IsTrue(result.Emails[0].BodyText.Contains("Body text"));
        Assert.IsTrue(result.Emails[0].From.Contains("a@frends.com"));
        Assert.IsTrue(result.Emails[0].To.Contains("b@frends.com"));
        Assert.IsTrue(result.Emails[0].Cc.Contains("c@frends.com"));
    }

    [Test]
    public void ReadEmail_SavesAttachments()
    {
        var raw = "From: a@frends.com\r\nTo: b@frends.com\r\nSubject: Att\r\nMessage-Id: <2@frends.com>\r\n" +
                  "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=\"b\"\r\n\r\n" +
                  "--b\r\nContent-Type: text/plain\r\n\r\nHi\r\n" +
                  "--b\r\nContent-Type: text/plain; name=\"a.txt\"\r\nContent-Disposition: attachment; filename=\"a.txt\"\r\n\r\ndata\r\n" +
                  "--b--\r\n";
        using var server = new FakeImapServer(new List<string> { raw });
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var connection = new Connection
            {
                UserName = "user",
                Password = "password",
                Host = "127.0.0.1",
                Port = server.Port,
                UseSSL = false,
                AcceptAllCerts = true,
            };
            var options = new Options
            {
                SaveAttachments = true,
                CreateDirectoryIfNotFound = true,
                SavedAttachmentsDirectory = dir,
            };

            var result = IMAP.ReadEmail(new Input { MaxEmails = 5 }, connection, options, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Emails[0].SavedAttachmentsPaths.Count);
            Assert.IsTrue(File.Exists(result.Emails[0].SavedAttachmentsPaths[0]));
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }

    [Test]
    public void SaveAttachments_ThrowsWhenDirectoryMissingAndAutocreateDisabled()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Assert.Throws<InvalidOperationException>(() =>
            IMAP.SaveMessageAttachments(missing, false, _dummyMessage));
    }

    [Test]
    public void SaveAttachments_SavesAttachedMessage()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var message = new MimeMessage { MessageId = "msg" };
            var multipart = new Multipart("mixed");
            multipart.Add(new TextPart("plain") { Text = "text" });
            multipart.Add(new MessagePart { Message = _dummyMessage, ContentDisposition = new ContentDisposition(ContentDisposition.Attachment) });
            message.Body = multipart;
            var paths = IMAP.SaveMessageAttachments(dir, true, message);
            Assert.AreEqual(1, paths.Count);
            Assert.IsTrue(File.Exists(paths[0]));
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }
}
