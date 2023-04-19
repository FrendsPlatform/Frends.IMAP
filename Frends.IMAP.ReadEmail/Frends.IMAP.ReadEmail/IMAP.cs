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
        /// list<string> AttachmentSaveDirs.
        /// }
        /// </returns>
        /// 

        public static List<EmailMessageResult> ReadEmail([PropertyTab] IMAPSettings settings, [PropertyTab] IMAPOptions options)
        {
            var result = new List<EmailMessageResult>();
            //additional list for attachments, not to modify EmailMessageResult
            var attachments = new List<IEnumerable<MimeEntity>>();

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
                        AttachmentSaveDirs = new List<string>()
                    });
                    attachments.Add(msg.Attachments);

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

                    //if directory exists, copy attachments there
                    if(_exist){
                        
                        for (int i = 0; i < attachments.Count; i++)
                        {
                            if(attachments[i].Count() > 0)
                            {
                                string directoryName = settings.ArchiveDirectory+"/"+result[i].Date.ToString("yyyy-MM-dd HH:mm:ss");
                                Directory.CreateDirectory(directoryName);

                                foreach (var attachment in attachments[i]) {
                                    string _directory = directoryName+"/";
                                    
                                    if (attachment is MessagePart) {
                                        var fileName = attachment.ContentDisposition?.FileName;
                                        var rfc822 = (MessagePart) attachment;
                                        _directory += fileName;

                                        if (string.IsNullOrEmpty (fileName))
                                            fileName = "attached-message.eml";

                                        using (var stream = File.Create (_directory))
                                            rfc822.Message.WriteTo (stream);
                                    }
                                    else{
                                        var part = (MimePart) attachment;
                                        var fileName = part.FileName;
                                        _directory += fileName;

                                        using (var stream = File.Create (_directory))
                                            part.Content.DecodeTo (stream);
                                    }
                                    
                                    result[i].AttachmentSaveDirs.Add(_directory);
                                }
                            }
                        }
                        
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