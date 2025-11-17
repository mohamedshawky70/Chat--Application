namespace ChatApplication.API.Entites;

public class ChatRoomUser
{
	public int ChatRoomId { get; set; } 
    public ChatRoom ChatRoom { get; set; } = null!;
    public string UserId { get; set; } = null!;
	public User User { get; set; } = null!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; }
}
