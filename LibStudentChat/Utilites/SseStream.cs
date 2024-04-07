namespace LibStudentChat.Utilites;

internal class SseEnumerator : IAsyncEnumerator<SseEvent>, IDisposable
{
    private readonly Stream baseStream;
    private readonly StreamReader reader;

    public SseEvent? Current { get; set; }

    public SseEnumerator(Stream sseStream)
    {
        baseStream = sseStream;
        reader = new StreamReader(baseStream);
    }

    public void Dispose()
    {
        baseStream.Dispose();
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        const string startEvent = "event: ";
        const string startData = "data: ";

        var line = await reader.ReadLineAsync();
        if (line is null)
            return false;

        if (!line.StartsWith(startEvent))
            throw new Exception("意外的异常");
        var eventName = line[startEvent.Length..];

        line = await reader.ReadLineAsync();
        if (line is null)
            throw new Exception("未经结束的异常");
        if (!line.StartsWith(startData))
            throw new Exception("意外的异常");
        var data = line[startData.Length..];

        line = await reader.ReadLineAsync();
        if (line != string.Empty)
            throw new Exception("意外的异常");

        Current = new SseEvent(eventName, data);
        return true;
    }

    public ValueTask DisposeAsync()
    {
        return baseStream.DisposeAsync();
    }
}

internal record SseEvent(string EventName, string Data);