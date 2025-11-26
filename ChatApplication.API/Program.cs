using ChatApplication.API;
using ChatApplication.API.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Add Serilog
builder.Host.UseSerilog((context,configuration) =>
	configuration.ReadFrom.Configuration(context.Configuration) //Read from appsettings.json
);

// Configure custom services
builder.Services.ConfigureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));

}
//Add Serilog 
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

//URl hub connection
app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.UseExceptionHandler();

app.Run();
