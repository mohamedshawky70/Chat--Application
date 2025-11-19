using ChatApplication.API.Entites;
using ChatApplication.API.enums;
using ChatApplication.API.Mapping;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.API.Hubs;

public class ChatHub(ApplicationDbContext context):Hub
{
	private readonly ApplicationDbContext _context = context;
	private readonly Dictionary<string, string> _connections = [];
	private readonly Dictionary<string, HashSet<string>> _UserConnections = [];// The same user can have multiple connections (multiple tabs)
	public override async Task OnConnectedAsync()
	{
		//Retrieves userId from the query string
		var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
		var connectionId = Context.ConnectionId;


		var user=new User();
		if(!string.IsNullOrEmpty(userId))
		{
			_connections.Add(connectionId, userId);

			if(_UserConnections.TryGetValue(userId, out HashSet<string>? value))
				value.Add(connectionId);
			else
				_UserConnections[userId] = new HashSet<string> { connectionId };

			user = await _context.Users.FindAsync(userId);
		}

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
		if (_connections.TryGetValue(Context.ConnectionId, out string? userId))
		{
			_connections.Remove(Context.ConnectionId);

			var user =await _context.Users.FindAsync(userId);
			if(user!=null)
			{
				user.ConnectionId = null;
				user.IsOnline = false;
				await _context.SaveChangesAsync();

				await Clients.All.SendAsync("UserStatusChanged", user.Id, user.MapToUserResponse());
			}
		
		}
		await base.OnDisconnectedAsync(exception);
	}

	//SendPrivateMessage
	public async Task SendPrivateMessage(string ReceiverId, string content)
	{
		if (!_connections.TryGetValue(Context.ConnectionId, out string? SenderId))
			return;

		var message = new Message
		{
			Content = content,
			SenderId = SenderId!,
			ReceiverId = ReceiverId,
			SentAt = DateTime.UtcNow,
			Type = MessageType.Text
		};
		_context.Messages.Add(message);
		await _context.SaveChangesAsync();
		await Clients.Caller.SendAsync("ReceivePrivateMessage", message.MapToMessageResponse());
		
	}

	//SendRoomMessage
	public async Task SendRoomMessage(int RoomId, string content)
	{
		if (!_connections.TryGetValue(Context.ConnectionId, out string? SenderId))
			return;

		var message = new Message
		{
			Content = content,
			SenderId= SenderId!,
			ChatRoomId = RoomId,
			SentAt = DateTime.UtcNow,
			Type = MessageType.Text
		};
		_context.Messages.Add(message);
		await _context.SaveChangesAsync();
		await Clients.Group($"Room_{RoomId}").SendAsync("ReceiveRoomMessage", message.MapToMessageResponse());
	}

	//JoinRoom
	public async Task JoinRoom(int roomId)
	{
		if (!_connections.TryGetValue(Context.ConnectionId, out string? userId))
			return;

		var user=await _context.Users.FindAsync(userId);
		var room= await _context.ChatRooms.FindAsync(roomId);
		if(user == null || room==null) 
			return;

		await Groups.AddToGroupAsync(Context.ConnectionId, $"Room_{roomId}");
		await Clients.All.SendAsync("UserJoinedRoom", roomId,new
		{
			RoomId = roomId,
			RoomName=room.Name,
			UserName=$"{user.FirstName} {user.LastName}",
			UserId = userId
		});
	}

	//LeaveRoom
	public async Task LeaveRoom(int roomId)
	{
		if (!_connections.TryGetValue(Context.ConnectionId, out string? userId))
			return;

		var user=await _context.Users.FindAsync(userId);
		var room= await _context.ChatRooms.FirstOrDefaultAsync(r=>r.Id==roomId);
		if(user == null || room==null) 
			return;

		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
		await Clients.All.SendAsync("UserLeftRoom",roomId,new
		{
			RoomId = roomId,
			RoomName=room.Name,
			UserName=$"{user.FirstName} {user.LastName}",
			UserId = userId
		});
	}

	//UserTyping
	public async Task UserTyping(string? receiveId,int? roomId)
	{
		if (!_connections.TryGetValue(Context.ConnectionId, out string? senderId))
			return;

		var user=await _context.Users.FindAsync(senderId);
		if(user==null)
			return;
		if (_UserConnections.TryGetValue(Context.ConnectionId, out var connections))
		{
			//To receivers
			foreach (var connectionId in connections)
			{
				await Clients.Client(connectionId).SendAsync("UserIsTyping",new
				{
					UserName=$"{user.FirstName} {user.LastName}",
					userId=senderId
				});
			}
		}
		else if(roomId.HasValue)
		{
			//To room
			await Clients.OthersInGroup($"room_{roomId}").SendAsync("UserIsTyping",new
			{
				UserName=$"{user.FirstName} {user.LastName}",
				UserId=senderId,
				RoomId=roomId
			});
		}
	}

	//MarkMessageAsRead
	public async Task MarkMessageAsRead(int messageId)
	{
		var message=await _context.Messages.FindAsync(messageId);
		if (message != null)
		{
			message.IsRead=true;
			await _context.SaveChangesAsync();
		}
		if (_UserConnections.TryGetValue(Context.ConnectionId, out var connections))
		{
			foreach (var connectionId in connections)
			{
				await Clients.Client(connectionId).SendAsync("MessageRead",messageId);
			}
		}
	}
}
