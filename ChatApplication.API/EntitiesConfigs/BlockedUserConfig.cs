using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class BlockedUserConfig : IEntityTypeConfiguration<BlockedUser>
{
	public void Configure(EntityTypeBuilder<BlockedUser> builder)
	{
		builder.HasOne(bu=>bu.Blocker)
			.WithMany(u=>u.BlockerUsers)
			.HasForeignKey(bu=>bu.BlockerId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(bu=>bu.Blocked)
			.WithMany(u=>u.BlockedUsers)
			.HasForeignKey(bu=>bu.BlockedId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
