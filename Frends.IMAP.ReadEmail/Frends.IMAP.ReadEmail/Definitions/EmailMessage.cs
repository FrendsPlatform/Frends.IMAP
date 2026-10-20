using System;
using System.Collections.Generic;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Output result for read operation
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Email id
    /// </summary>
    /// <example>abc123@mail.example.com</example>
    public string Id { get; set; }

    /// <summary>
    /// To-field from email
    /// </summary>
    /// <example>jane.doe@example.com</example>
    public string To { get; set; }

    /// <summary>
    /// Cc-field from email
    /// </summary>
    /// <example>john.smith@example.com</example>
    public string Cc { get; set; }

    /// <summary>
    /// From-field from email
    /// </summary>
    /// <example>sender@example.com</example>
    public string From { get; set; }

    /// <summary>
    /// Email received date
    /// </summary>
    /// <example>2026-01-15T10:30:00</example>
    public DateTime Date { get; set; }

    /// <summary>
    /// Title of the email
    /// </summary>
    /// <example>Monthly report</example>
    public string Subject { get; set; }

    /// <summary>
    /// Body of the email as text
    /// </summary>
    /// <example>Hello, please find the report attached.</example>
    public string BodyText { get; set; }

    /// <summary>
    /// Body html is available
    /// </summary>
    /// <example>&lt;p&gt;Hello, please find the report attached.&lt;/p&gt;</example>
    public string BodyHtml { get; set; }

    /// <summary>
    /// Attachment download path
    /// </summary>
    /// <example>["c:/SavedAttachments/abc123/report.pdf"]</example>
    public List<string> SavedAttachmentsPaths { get; set; }
}
