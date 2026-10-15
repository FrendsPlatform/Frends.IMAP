using System.Collections.Generic;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Result of reading emails from IMAP server.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// List of emails that were retrieved. Null when Success is false.
    /// </summary>
    /// <example>[ { Id = "abc123", Subject = "Monthly report" } ]</example>
    public List<EmailMessage> Emails { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success, List<EmailMessage> emails = null, Error error = null)
    {
        Success = success;
        Emails = emails;
        Error = error;
    }
}
