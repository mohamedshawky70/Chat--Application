using ChatApplication.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.API;

public static class DependencyInjection
{
	public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
	{
		//Add Database
		var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(connectionString));
		//Add IdentityRole
		services.AddIdentity<User, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();


		




		return services;


	}
}
