using System.ComponentModel;

namespace Frends.IMAP.ReadEmail
{
    /// <summary>
    /// Options related to IMAP reading
    /// </summary>
    public class IMAPOptions
    {
        /// <summary>
        /// Maximum number of emails to retrieve
        /// </summary>
        [DefaultValue(10)]
        public int MaxEmails { get; set; }

        /// <summary>
        /// Should get only unread emails?
        /// </summary>
        public bool GetOnlyUnreadEmails { get; set; }

        /// <summary>
        /// If true, then marks queried emails as read
        /// </summary>
        public bool MarkEmailsAsRead { get; set; }

        /// <summary>
        /// If true, then received emails will be hard deleted
        /// </summary>
        public bool DeleteReadEmails { get; set; }

        /// <summary>
        /// If true, then creates an attachment directory if not existing
        /// </summary>
        [DefaultValue(false)]
        public bool CreateDirectoryIfNotFound { get; set; }
    }
}