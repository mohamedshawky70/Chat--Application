using ChatApplication.API.Mapping;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.API.Hubs;

public class ChatHub(ApplicationDbContext context):Hub
{
	private readonly ApplicationDbContext _context = context;
	public override async Task OnConnectedAsync()
	{
		//Retrieves userId from the query string
		var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
		var connectionId = Context.ConnectionId;

		var user=new User();
		if(!string.IsNullOrEmpty(userId))
			user = await _context.Users.FindAsync(userId);

		if (user != null)
		{
			user.ConnectionId = connectionId;
			user.IsOnline = true;
			await _context.SaveChangesAsync();

			await Clients.All.SendAsync("UserStatusChanged", userId, user.MapToUserResponse());
		}
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var user = _context.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
		if (user != null)
		{
			user.ConnectionId = null;
			user.IsOnline = false;
			await _context.SaveChangesAsync();

			await Clients.All.SendAsync("UserStatusChanged", user.Id, user.MapToUserResponse());
		}
		await base.OnDisconnectedAsync(exception);
	}
}
