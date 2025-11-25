using Microsoft.AspNetCore.Identity;

namespace ChatApplication.API.Entites;

public class User: IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeen { get; set; } 
    public bool IsOnline { get; set; }
    public bool IsDeleted { get; set; }
    public string? ConnectionId { get; set; }
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Message> ReceivedMessages { get; set; } = [];
    public ICollection<Message> PinnedMessages { get; set; } = [];
    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = [];
    public ICollection<BlockedUser> BlockerUsers { get; set; } = [];
    public ICollection<BlockedUser> BlockedUsers { get; set; } = [];
}
