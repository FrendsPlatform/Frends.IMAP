using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Settings for IMAP and POP3 servers
/// </summary>
public class Settings
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
    /// <example>true</example>
    [DefaultValue(true)]
    public bool UseSSL { get; set; }

    /// <summary>
    /// Controls whether task will save attachments to designated directory or not
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool AcceptAllCerts { get; set; }

    /// <summary>
    /// Account name to login with
    /// </summary>
    /// <example>user</example>
    [DefaultValue("accountName")]
    [DisplayFormat(DataFormatString = "Text")]
    public string UserName { get; set; }

    /// <summary>
    /// Account password
    /// </summary>
    /// <example>***</example>
    [PasswordPropertyText]
    public string Password { get; set; }
}