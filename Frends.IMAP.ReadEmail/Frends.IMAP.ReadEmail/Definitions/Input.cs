using System.ComponentModel;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Input parameters for reading emails.
/// </summary>
public class Input
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
}
