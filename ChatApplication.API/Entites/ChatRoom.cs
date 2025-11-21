namespace ChatApplication.API.Entites;

public class ChatRoom
{
	public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPrivate { get; set; }
    public string CreatorUserId { get; set; } =null!;
	public ICollection<Message> Messages { get; set; } = [];
    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = [];
}
