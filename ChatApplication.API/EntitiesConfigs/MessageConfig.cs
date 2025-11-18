using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
	public void Configure(EntityTypeBuilder<Message> builder)
	{

		builder.Property(m => m.Content)
			.HasMaxLength(900);

		builder.HasOne(m => m.Sender)
			.WithMany(u => u.SentMessages)
			.HasForeignKey(m => m.SenderId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(m => m.Receiver)
			.WithMany(u => u.ReceivedMessages)
			.HasForeignKey(m => m.ReceiverId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(m => m.ChatRoom)
			.WithMany(c => c.Messages)
			.HasForeignKey(m => m.ChatRoomId)
			.OnDelete(DeleteBehavior.Cascade);


	}
}
