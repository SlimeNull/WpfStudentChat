namespace LibStudentChat;

public class ChatClient
{
    public ChatClient(Uri baseUri)
    {
        BaseUri = baseUri;
    }

    public Uri BaseUri { get; }

    
    public async Task LoginAsync(string username, string password)
    {

    }

}
