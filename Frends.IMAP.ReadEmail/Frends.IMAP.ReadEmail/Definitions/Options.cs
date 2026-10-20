using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Options related to IMAP reading
/// </summary>
public class Options
{
    /// <summary>
    /// If true, then marks queried emails as read
    /// </summary>
    /// <example>false</example>
    public bool MarkEmailsAsRead { get; set; }

    /// <summary>
    /// If true, then received, read emails will be hard deleted.
    /// Note: If MarkEmailsAsRead is false, then this option will delete only those emails that were already read.
    /// </summary>
    /// <example>false</example>
    public bool DeleteReadEmails { get; set; }

    /// <summary>
    /// Controls whether task will save attachments to designated directory or not
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool SaveAttachments { get; set; }

    /// <summary>
    /// Directory to store all attachments
    /// </summary>
    /// <example>c:/SavedAttachments</example>
    [UIHint(nameof(SaveAttachments), "", true)]
    [DefaultValue("c:/SavedAttachments")]
    [DisplayFormat(DataFormatString = "Text")]
    public string SavedAttachmentsDirectory { get; set; }

    /// <summary>
    /// If true, then creates an attachment directory if not existing
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(SaveAttachments), "", true)]
    [DefaultValue(false)]
    public bool CreateDirectoryIfNotFound { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure. If `ThrowErrorOnFailure` is set to `true`, then the original exception will be wrapped in a new Exception with this error message.
    /// </summary>
    /// <example>Reading emails failed: mailbox unavailable</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
