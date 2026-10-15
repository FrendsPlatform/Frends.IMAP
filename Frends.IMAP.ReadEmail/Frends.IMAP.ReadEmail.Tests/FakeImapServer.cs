using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frends.IMAP.ReadEmail.Tests;

/// <summary>
/// Minimal in-process plain text IMAP server used to test the read flow without external services.
/// </summary>
internal sealed class FakeImapServer : IDisposable
{
    private readonly TcpListener _listener;
    private readonly List<string> _messages;
    private readonly Task _acceptLoop;

    public FakeImapServer(List<string> messages)
    {
        _messages = messages;
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        _acceptLoop = Task.Run(AcceptAsync);
    }

    public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

    public List<string> Commands { get; } = new();

    public void Dispose()
    {
        _listener.Stop();
        try
        {
            _acceptLoop.Wait(1000);
        }
        catch (AggregateException)
        {
        }
    }

    private async Task AcceptAsync()
    {
        try
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleAsync(client));
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException or SocketException)
        {
        }
    }

    private async Task HandleAsync(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream, Encoding.ASCII))
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\r\n", AutoFlush = true })
        {
            await writer.WriteLineAsync("* OK IMAP4rev1 ready");
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lock (Commands) Commands.Add(line);
                var parts = line.Split(' ', 3);
                if (parts.Length < 2) continue;
                var tag = parts[0];
                var cmd = parts[1].ToUpperInvariant();
                var args = parts.Length > 2 ? parts[2] : string.Empty;
                if (cmd == "UID")
                {
                    var sub = args.Split(' ', 2);
                    cmd = sub[0].ToUpperInvariant();
                    args = sub.Length > 1 ? sub[1] : string.Empty;
                }

                switch (cmd)
                {
                    case "CAPABILITY":
                        await writer.WriteLineAsync("* CAPABILITY IMAP4rev1");
                        await writer.WriteLineAsync($"{tag} OK done");
                        break;
                    case "LIST":
                        if (args.Contains("INBOX"))
                            await writer.WriteLineAsync("* LIST (\\HasNoChildren) \"/\" \"INBOX\"");
                        await writer.WriteLineAsync($"{tag} OK done");
                        break;
                    case "SELECT":
                        await writer.WriteLineAsync($"* {_messages.Count} EXISTS");
                        await writer.WriteLineAsync("* FLAGS (\\Seen \\Deleted)");
                        await writer.WriteLineAsync("* OK [UIDVALIDITY 1] ok");
                        await writer.WriteLineAsync($"{tag} OK [READ-WRITE] done");
                        break;
                    case "SEARCH":
                        var ids = new List<string>();
                        for (var i = 1; i <= _messages.Count; i++) ids.Add(i.ToString());
                        await writer.WriteLineAsync($"* SEARCH {string.Join(" ", ids)}");
                        await writer.WriteLineAsync($"{tag} OK done");
                        break;
                    case "FETCH":
                        var id = int.Parse(args.Split(' ')[0]);
                        var bytes = Encoding.UTF8.GetBytes(_messages[id - 1]);
                        await writer.WriteLineAsync($"* {id} FETCH (UID {id} BODY[] {{{bytes.Length}}}");
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        await writer.WriteLineAsync(")");
                        await writer.WriteLineAsync($"{tag} OK done");
                        break;
                    case "LOGOUT":
                        await writer.WriteLineAsync("* BYE bye");
                        await writer.WriteLineAsync($"{tag} OK done");
                        return;
                    default:
                        await writer.WriteLineAsync($"{tag} OK done");
                        break;
                }
            }
        }
    }
}
