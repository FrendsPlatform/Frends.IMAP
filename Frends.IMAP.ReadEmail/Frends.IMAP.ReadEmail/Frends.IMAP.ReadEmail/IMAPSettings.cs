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
        [DefaultValue("imap.frends.com")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Host { get; set; }

        /// <summary>
        /// Host port
        /// </summary>
        [DefaultValue(993)]
        public int Port { get; set; }

        /// <summary>
        /// Use SSL or not
        /// </summary>
        [DefaultValue(true)]
        public bool UseSSL { get; set; }

        /// <summary>
        /// Should the task accept all certificates from IMAP server, including invalid ones?
        /// </summary>
        [DefaultValue(false)]
        public bool AcceptAllCerts { get; set; }

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