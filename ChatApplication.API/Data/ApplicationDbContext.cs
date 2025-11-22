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
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(builder);
	}
}
