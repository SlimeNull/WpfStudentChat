using System.Runtime.CompilerServices;

namespace LibStudentChat.Utilites;

public record class ServerSentEvent(string Event, string? Data, string? Id, string? Retry)
{
    public static async IAsyncEnumerable<ServerSentEvent> EnumerateFromStream(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const string PrefixEvent = "event: ";
        const string PrefixData = "data: ";
        const string PrefixId = "id: ";
        const string PrefixRetry = "retry: ";

        StreamReader reader = new(stream);
        ServerSentEvent current = new(string.Empty, null, null, null);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
            {
                yield break;
            }

            if (line.StartsWith(PrefixEvent, StringComparison.OrdinalIgnoreCase))
            {
                current = current with
                {
                    Event = line.Substring(PrefixEvent.Length)
                };
            }
            else if (line.StartsWith(PrefixData, StringComparison.OrdinalIgnoreCase))
            {
                current = current with
                {
                    Data = line.Substring(PrefixData.Length)
                };
            }
            else if(line.StartsWith(PrefixId, StringComparison.OrdinalIgnoreCase))
            {
                current = current with
                {
                    Id = line.Substring(PrefixId.Length)
                };
            }
            else if(line.StartsWith(PrefixRetry, StringComparison.OrdinalIgnoreCase))
            {
                current = current with
                {
                    Retry = line.Substring(PrefixRetry.Length)
                };
            }
            else if (string.IsNullOrWhiteSpace(line))
            {
                yield return current;
                current = new(string.Empty, null, null, null);
            }
        }
    }
}