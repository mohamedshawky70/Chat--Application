using ChatApplication.API.enums;

namespace ChatApplication.API.Entites;

public class Message
{
	public int Id { get; set; } 
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPinned { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;
    public string SenderId { get; set; } = null!;
    public User Sender { get; set; } = null!;  
    public string? ReceiverId { get; set; }
    public User? Receiver { get; set; }
    public string? PinnedId { get; set; }
    public User? PinnedBy { get; set; }
    public int? ChatRoomId { get; set; }
    public ChatRoom? ChatRoom { get; set; }
}
