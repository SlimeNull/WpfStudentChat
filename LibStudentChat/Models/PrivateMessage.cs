namespace StudentChat.Models;

public record class PrivateMessage : Message
{
    public int ReceiverId { get; set; }
}
