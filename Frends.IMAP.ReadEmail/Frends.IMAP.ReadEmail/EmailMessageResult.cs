using System;
using System.Collections.Generic;
using MimeKit;

namespace Frends.IMAP.ReadEmail
{
    /// <summary>
    /// Output result for read operation
    /// </summary>
    public class EmailMessageResult
    {
        /// <summary>
        /// Email id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// To-field from email
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Cc-field from email
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// From-field from email
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Email received date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title of the email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of the email as text
        /// </summary>
        public string BodyText { get; set; }

        /// <summary>
        /// Body html is available
        /// </summary>
        public string BodyHtml { get; set; }

        /// <summary>
        /// Attachment download path
        /// </summary>
        public List<string> AttachmentSaveDirs { get; set; }
    }
}