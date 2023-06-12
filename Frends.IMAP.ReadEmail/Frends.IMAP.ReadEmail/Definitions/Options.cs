using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Options related to IMAP reading
/// </summary>
public class Options
{
    /// <summary>
    /// Maximum number of emails to retrieve
    /// </summary>
    /// <example>10</example>
    [DefaultValue(10)]
    public int MaxEmails { get; set; }

    /// <summary>
    /// Should get only unread emails?
    /// </summary>
    /// <example>false</example>
    public bool GetOnlyUnreadEmails { get; set; }

    /// <summary>
    /// If true, then marks queried emails as read
    /// </summary>
    /// <example>false</example>
    public bool MarkEmailsAsRead { get; set; }

    /// <summary>
    /// If true, then received emails will be hard deleted
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
}
