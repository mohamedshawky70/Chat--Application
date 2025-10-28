using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace ChatApplication.API.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(builder);
	}
}
