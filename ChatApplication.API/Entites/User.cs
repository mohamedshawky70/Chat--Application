using Microsoft.AspNetCore.Identity;

namespace ChatApplication.API.Entites;

public class User: IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsOnline { get; set; }
    public string? ConnectionId { get; set; }
    public string? Avatar { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    //public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    //public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    //public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();
}
