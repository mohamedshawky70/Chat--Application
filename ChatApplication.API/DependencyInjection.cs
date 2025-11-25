using ChatApplication.API.Authentication;
using ChatApplication.API.Data;
using ChatApplication.API.Services.AccountService;
using ChatApplication.API.Services.EmailService;
using ChatApplication.API.Services.FileService;
using ChatApplication.API.Services.MessagesService;
using ChatApplication.API.Services.RoomService;
using ChatApplication.API.Services.UserService;
using ChatApplication.API.Settings;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;

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


		//Add IOptions
		services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

		//علشان يشغل الفليديشن علي الكلاس علشان ممكن اغلط وادخل قيم غير منطقيه زي وقت انتهاء التوكن بالسالب او مدخلهوش اصلا
		services.AddOptions<JwtSettings>().BindConfiguration(nameof(JwtSettings)).ValidateDataAnnotations();

		// علشان اعرف اجيب قيم الكلاس من السكشن علشان استخدمهم هنا
		var settings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

		//Add Authentication
		services.AddAuthentication(option =>
		{
			option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}
		)
		.AddJwtBearer(o =>
		{
			o.SaveToken = true;
			o.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Key!)),
				ValidIssuer = settings?.Issuer,
				ValidAudience = settings?.Audience,
			};
		});

		//Add Email Sender Service
		services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
		services.AddHttpContextAccessor();

		//Add Services
		services.AddScoped<IEmailSender, EmailService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IJwtProvider, JwtProvider>();
		services.AddScoped<IFileService, FileService>();
		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IUserServeic, UserService>();
		services.AddScoped<IMessagesService, MessagesService>();
		services.AddScoped<IRoomService, RoomService>();

		//Add Fluent Validation
		services.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

		//Add Hangfire
		services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
		services.AddHangfireServer();

		//Add SingnalR
		services.AddSignalR();

		//Add Cors
		var AllowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
		services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.WithOrigins(AllowedOrigins!) // Adjust the origin as needed
					   .AllowAnyHeader()
					   .AllowAnyMethod()
					   .AllowCredentials();
			});
		});

		//Add Exception Handler
		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();



		return services;

	}
}
