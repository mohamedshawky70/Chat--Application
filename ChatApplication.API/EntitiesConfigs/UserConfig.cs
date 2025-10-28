using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{	
		builder.Property(u => u.PhoneNumber)
			.HasMaxLength(13);// With country code

		builder.Property(u => u.ConnectionId)
			.HasMaxLength(200);
	}
}
