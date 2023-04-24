using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Linq.Expressions;

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
        /// List<string> AttachmentSaveDirs.
        /// }
        /// </returns>
        /// 

        //method to generate file paths
        private static string GenerateFilePath(MimeEntity attachment, string directoryName){
            var directory = $"{directoryName}/";
            var fileName = "";

            if (attachment is MessagePart) {
                fileName = attachment.ContentDisposition?.FileName;
                if (string.IsNullOrEmpty (fileName))
                    fileName = "attached-message.eml";
            }
            else{
                var part = (MimePart) attachment;
                fileName = part.FileName;
            }
            return $"{directory}/{fileName}";
        }

        //method to save attachments into directory
        private static void SaveAttachments(IMAPSettings settings,IMAPOptions options, List<EmailMessageResult> emails, IDictionary<string, IEnumerable<MimeEntity>> attachments){
            
            //check existence of directory
            bool exist = Directory.Exists(settings.SavedAttachmentsDirectory);

            //if not existing and set to create new, then proceed
            if(!exist){
                if(options.CreateDirectoryIfNotFound)
                    try{
                        Directory.CreateDirectory(settings.SavedAttachmentsDirectory);
                    }
                    catch{
                        throw;
                    }
                else{
                    throw new System.Exception("Directory not found, and automatic creation is disabled. Check 'IMAPSettings.SavedAttachmentsDirectory' for a valid path or consider enabling 'IMAPOptions.CreateDirectoryIfNotFound'");
                }
            }
            //saving attachemnts into designated directory
            try{
                foreach(var msg in attachments){
                    if(msg.Value.Any())
                    {
                        //find message with matching ID
                        var item = emails.Find(i => i.Id == msg.Key);
                        var directoryName = $"{settings.SavedAttachmentsDirectory}/{item.Date.ToString("s")}";
                        Directory.CreateDirectory(directoryName);
                     foreach (var attachment in msg.Value) {
                            var path = GenerateFilePath(attachment, directoryName);
                         if (attachment is MessagePart) {
                                var part = (MessagePart) attachment;
                             using (var stream = File.Create (path))
                                    part.Message.WriteTo (stream);
                            }
                            else{
                                var part = (MimePart) attachment;
                             using (var stream = File.Create (path))
                                    part.Content.DecodeTo (stream);
                            }
                            item.SavedAttachmentsPaths.Add(path);
                        }
                    }
                }
            }
            catch{
                throw;
            }
        }

        public static List<EmailMessageResult> ReadEmail([PropertyTab] IMAPSettings settings, [PropertyTab] IMAPOptions options)
        {
            var result = new List<EmailMessageResult>();
            //additional dictionary for attachments, not to modify EmailMessageResult - stores message ID and attachments
            IDictionary<string,IEnumerable<MimeEntity>> attachments = new Dictionary<string, IEnumerable<MimeEntity>>();

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
                        SavedAttachmentsPaths = new List<string>()
                    });

                    if(msg.Attachments.Any())
                        attachments.Add(msg.MessageId, msg.Attachments);

                    // should mark emails as read?
                    if (!options.DeleteReadEmails && options.MarkEmailsAsRead)
                    {
                        inbox.AddFlags(messageIds[i], MessageFlags.Seen, true);
                    }
                }

                // save attachments?
                if (settings.SaveAttachments)
                {
                   SaveAttachments(settings, options,result, attachments);
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