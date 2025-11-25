using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApplication.API.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<ChatRoom> ChatRooms { get; set; }
	public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<UploadFile>  UploadFiles { get; set; }
	public DbSet<BlockedUser>  BlockedUsers { get; set; }
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		//Global query filter
		builder.Entity<Message>().HasQueryFilter(u => !u.IsDeleted);
		base.OnModelCreating(builder);
	}

	override public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var entries = ChangeTracker
			.Entries()
			.Where(e => e.Entity is User && 
				(e.State == EntityState.Added || e.State == EntityState.Modified));
		foreach (var entityEntry in entries)
		{
			if (entityEntry.State == EntityState.Added)
				((User)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
		}
		return base.SaveChangesAsync(cancellationToken);
	}
}
