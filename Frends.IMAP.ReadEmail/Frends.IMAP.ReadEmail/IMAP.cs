using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Frends.IMAP.ReadEmail.Definitions;

namespace Frends.IMAP.ReadEmail;

public class IMAP
{
    /// <summary>
    /// Frends task, that allows to read emails from IMAP server and 
    /// optionally save attachments to designated directory. The result of 
    /// the task contains a list of emails objects with message details
    /// and a list of saved attachment paths.
    ///
    /// Attachments are saved to a directory defined in options. For each
    /// email, a subdirectory is created. Subdirectory name is the same as
    /// email message id.
    ///
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.IMAP.ReadEmail)
    /// </summary>
    /// <param name="settings">IMAP server settings</param>
    /// <param name="options">Email options</param>
    /// <returns>
    /// object 
    /// {
    ///     List&lt;EmailMessageResult&gt; Emails
    ///     {
    ///         string Id,
    ///         string To,
    ///         string Cc,
    ///         string From,
    ///         DateTime Date,
    ///         string Subject,
    ///         string BodyText,
    ///         string BodyHtml,
    ///         List&lt;string&gt; AttachmentSaveDirs
    ///     }
    /// }
    /// </returns>
    public static Result ReadEmail(
        [PropertyTab] IMAPSettings settings,
        [PropertyTab] IMAPOptions options)
    {
        var result = new List<EmailMessageResult>();
        using var client = new ImapClient();

        // Accept all certs?
        if (settings.AcceptAllCerts)
        {
            client.ServerCertificateValidationCallback = 
                (s, x509certificate, x590chain, sslPolicyErrors) => true;
        }

        client.Connect(settings.Host, settings.Port, settings.UseSSL);
        client.Authenticate(settings.UserName, settings.Password);

        var inbox = client.Inbox;
        inbox.Open(FolderAccess.ReadWrite);

        // Get all or only unread emails?
        IList<UniqueId> messageIds = options.GetOnlyUnreadEmails
            ? inbox.Search(SearchQuery.NotSeen)
            : inbox.Search(SearchQuery.All);

        // Read as many as there are unread emails or as many as defined in 
        // options.MaxEmails.
        for (int i = 0; i < messageIds.Count && i < options.MaxEmails; i++)
        {
            MimeMessage msg = inbox.GetMessage(messageIds[i]);

            var savedAttachmentPaths = new List<string>();
            if (options.SaveAttachments)
            {
                savedAttachmentPaths = SaveMessageAttachments(
                    options.SavedAttachmentsDirectory,
                    options.CreateDirectoryIfNotFound,
                    msg);
            }

            result.Add(new EmailMessageResult
            {
                Id = msg.MessageId,
                Date = msg.Date.DateTime,
                Subject = msg.Subject,
                BodyText = msg.TextBody,
                BodyHtml = msg.HtmlBody,
                From = string.Join(",", msg.From.Select(j => j.ToString())),
                To = string.Join(",", msg.To.Select(j => j.ToString())),
                Cc = string.Join(",", msg.Cc.Select(j => j.ToString())),
                SavedAttachmentsPaths = savedAttachmentPaths
            });

            // Should mark emails as read?
            if (!options.DeleteReadEmails && options.MarkEmailsAsRead)
            {
                inbox.AddFlags(messageIds[i], MessageFlags.Seen, true);
            }
        }

        // Should delete emails?
        if (options.DeleteReadEmails && messageIds.Any())
        {
            inbox.AddFlags(messageIds, MessageFlags.Deleted, false);
            inbox.Expunge();
        }

        client.Disconnect(true);
    
        return new Result(result);
    }

    internal static string GenerateAttachmentFilePath(
        MimeEntity attachment,
        string attachmentsDirectoryPath)
    {
        var fileName = "";

        if (attachment is MessagePart)
        {
            fileName = attachment.ContentDisposition?.FileName;
            if (string.IsNullOrEmpty(fileName))
                fileName = $"attached-message{Guid.NewGuid()}.eml";
        }
        else
        {
            var part = (MimePart)attachment;
            fileName = part.FileName;
        }
        return $"{attachmentsDirectoryPath}/{fileName}";
    }

    internal static List<string> SaveMessageAttachments(
        string directory, bool createDir, MimeMessage message)
    {
        var result = new List<string>();

        if (!message.Attachments.Any())
            return result;

        bool directoryExists = Directory.Exists(directory);

        if (!directoryExists)
        {
            if (createDir)
                Directory.CreateDirectory(directory);
            else
            {
                // Throw exception if directory not found,
                // and autocreation is turned off
                throw new InvalidOperationException(
                    $"Directory '{directory}' not found, and automatic " +
                    "creation is disabled. Check 'SavedAttachmentsDirectory' " +
                    "for a valid path or consider enabling " + 
                    "'CreateDirectoryIfNotFound'");
            }
        }

        // Local path to each email directory
        var directoryName = $"{directory}/{message.MessageId}";
        Directory.CreateDirectory(directoryName);

        foreach (var attachment in message.Attachments)
        {
            var path = GenerateAttachmentFilePath(attachment, directoryName);
            if (attachment is MessagePart)
            {
                var part = (MessagePart)attachment;
                using (var stream = File.Create(path))
                    part.Message.WriteTo(stream);
            }
            else
            {
                var part = (MimePart)attachment;
                using (var stream = File.Create(path))
                    part.Content.DecodeTo(stream);
            }
            result.Add(path);
        }

        return result;
    }
}
