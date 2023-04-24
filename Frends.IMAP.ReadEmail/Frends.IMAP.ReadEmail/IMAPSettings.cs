using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.IMAP.ReadEmail
{
    /// <summary>
    /// Settings for IMAP and POP3 servers
    /// </summary>
    public class IMAPSettings
    {
        /// <summary>
        /// Host address
        /// </summary>
        /// <example>imap.frends.com</example>
        [DefaultValue("imap.frends.com")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Host { get; set; }

        /// <summary>
        /// Host port
        /// </summary>
        /// <example>993</example>
        [DefaultValue(993)]
        public int Port { get; set; }

        /// <summary>
        /// Use SSL or not
        /// </summary>
        [DefaultValue(true)]
        public bool UseSSL { get; set; }

        /// <summary>
        /// Controls whether task will save attachments to designated directory or not
        /// </summary>
        [DefaultValue(false)]
        public bool AcceptAllCerts { get; set; }

        /// <summary>
        /// Should the task save attachments into designated directory?
        /// </summary>
        [DefaultValue(false)]
        public bool SaveAttachments { get; set; }

        /// <summary>
        /// Directory to store all attachments
        /// </summary>
        /// <example>c:/SavedAttachments</example>
        [DefaultValue("c:/SavedAttachments")]
        [DisplayFormat(DataFormatString = "Text")]
        public string SavedAttachmentsDirectory { get; set; }

        /// <summary>
        /// Account name to login with
        /// </summary>
        [DefaultValue("accountName")]
        [DisplayFormat(DataFormatString = "Text")]
        public string UserName { get; set; }

        /// <summary>
        /// Account password
        /// </summary>
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}