namespace ChatApplication.API.Entites;

public class BlockedUser
{
	public int Id { get; set; }
	public string BlockerId { get; set; } = null!;
	public User Blocker { get; set; } = null!;
	public string BlockedId { get; set; } = null!;
	public User Blocked { get; set; } = null!;
	public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
}
