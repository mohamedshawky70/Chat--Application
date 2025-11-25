//Start:27/10/2025
//End: /11/2025

using ChatApplication.API;
using ChatApplication.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

//URl hub connection
app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.UseExceptionHandler();

app.Run();
