using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class ChatRoomConfig : IEntityTypeConfiguration<ChatRoom>
{
	public void Configure(EntityTypeBuilder<ChatRoom> builder)
	{

		builder.Property(cr => cr.Name)
			.HasMaxLength(200);

		builder.Property(cr => cr.Description)
			.HasMaxLength(500);
	}
}
