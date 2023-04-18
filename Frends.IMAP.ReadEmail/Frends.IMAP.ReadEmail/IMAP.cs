using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;

namespace Frends.IMAP.ReadEmail
{
    public class IMAP
    {
        /// <summary>
        /// Read emails from IMAP server.
        /// [Documentation](https://github.com/FrendsPlatform/Frends.IMAP/tree/main/Frends.IMAP.ReadEmail)
        /// </summary>
        /// <param name="settings">IMAP server settings</param>
        /// <param name="options">Email options</param>
        /// <returns>
        /// List of
        /// {
        /// string Id.
        /// string To.
        /// string Cc.
        /// string From.
        /// DateTime Date.
        /// string Subject.
        /// string BodyText.
        /// string BodyHtml.
        /// }
        /// </returns>
        /// 

        public static List<EmailMessageResult> ReadEmail([PropertyTab] IMAPSettings settings, [PropertyTab] IMAPOptions options)
        {
            var result = new List<EmailMessageResult>();

            using (var client = new ImapClient())
            {
                // accept all certs?
                if (settings.AcceptAllCerts)
                {
                    client.ServerCertificateValidationCallback = (s, x509certificate, x590chain, sslPolicyErrors) => true;
                }


                // connect to imap server
                client.Connect(settings.Host, settings.Port, settings.UseSSL);

                // authenticate with imap server
                client.Authenticate(settings.UserName, settings.Password);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                // get all or only unread emails?
                IList<UniqueId> messageIds = options.GetOnlyUnreadEmails
                    ? inbox.Search(SearchQuery.NotSeen)
                    : inbox.Search(SearchQuery.All);

                // read as many as there are unread emails or as many as defined in options.MaxEmails
                for (int i = 0; i < messageIds.Count && i < options.MaxEmails; i++)
                {
                    MimeMessage msg = inbox.GetMessage(messageIds[i]);
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
                        Attachments = msg.Attachments
                    });

                    // should mark emails as read?
                    if (!options.DeleteReadEmails && options.MarkEmailsAsRead)
                    {
                        inbox.AddFlags(messageIds[i], MessageFlags.Seen, true);
                    }
                }

                // save attachments?
                if (settings.SaveAttachments && !string.IsNullOrEmpty(settings.ArchiveDirectory))
                {
                    //check existence of directory
                    bool _exist = Directory.Exists(settings.ArchiveDirectory);

                    //if not existing and set to create new, then proceed
                    if(!_exist && options.CreateDirectoryIfNotFound)
                        Directory.CreateDirectory(settings.ArchiveDirectory);
                    
                    //check again, creation might not have worked
                    _exist = Directory.Exists(settings.ArchiveDirectory);

                    //if exists, do stuff
                    if(_exist){
                        foreach(var msg in result)
                        {
                            foreach (var attachment in msg.Attachments) {
                                if (attachment is MessagePart) {
                                    var fileName = attachment.ContentDisposition?.FileName;
                                    var rfc822 = (MessagePart) attachment;

                                    if (string.IsNullOrEmpty (fileName))
                                        fileName = "attached-message.eml";

                                    using (var stream = File.Create (settings.ArchiveDirectory+"/"+fileName))
                                        rfc822.Message.WriteTo (stream);
                                } else {
                                    var part = (MimePart) attachment;
                                    var fileName = part.FileName;

                                    using (var stream = File.Create (settings.ArchiveDirectory+"/"+fileName))
                                        part.Content.DecodeTo (stream);
                                }
                            }                       
                        }
                    }
                    else{

                    }

                }

                // should delete emails?
                if (options.DeleteReadEmails && messageIds.Any())
                {
                    inbox.AddFlags(messageIds, MessageFlags.Deleted, false);
                    inbox.Expunge();
                }

                client.Disconnect(true);
            }

            return result;
        }
    }
}