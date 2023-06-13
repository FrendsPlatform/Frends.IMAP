using System.Collections.Generic;

namespace Frends.IMAP.ReadEmail.Definitions;

/// <summary>
/// Result of reading emails from IMAP server.
/// </summary>
public class Result
{
    /// <summary>
    /// List of emails that were retrieved.
    /// </summary>
    public List<EmailMessage> Emails { get; private set; }

    internal Result(List<EmailMessage> emails)
    {
        Emails = emails;
    }
}