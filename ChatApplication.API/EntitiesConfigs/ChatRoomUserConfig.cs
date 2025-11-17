using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class ChatRoomUserConfig : IEntityTypeConfiguration<ChatRoomUser>
{
	public void Configure(EntityTypeBuilder<ChatRoomUser> builder)
	{
		builder.HasKey(cru => new { cru.ChatRoomId, cru.UserId });
	}
}

