// See https://aka.ms/new-console-template for more information
using StudentChat;

Console.WriteLine("Hello, World!");

ChatClient client = new(new Uri("http://localhost:5270", UriKind.Absolute));

client.PrivateMessageReceived += Client_PrivateMessageReceived;

await client.LoginAsync("Test", "TestHash");
Console.WriteLine("Login ok");


await Task.Delay(-1);

void Client_PrivateMessageReceived(object? sender, StudentChat.Models.Events.PrivateMessageReceivedEventArgs e)
{
    Console.WriteLine($"Message received: {e.Message.Content}");
}