using ChatApplication.API.Common;
using ChatApplication.API.DTOs.Message;
using ChatApplication.API.DTOs.Room;
using ChatApplication.API.Hubs;
using ChatApplication.API.Mapping;
using MimeKit;

namespace ChatApplication.API.Services.RoomService;

public class RoomService(ApplicationDbContext context, IHubContext<ChatHub> hubContext) : IRoomService
{
	private readonly ApplicationDbContext _context = context;
	private readonly IHubContext<ChatHub> _hubContext = hubContext;
	public async Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
	{
		var rooms = await _context.ChatRooms
			.Include(cr => cr.ChatRoomUsers)
			.ToListAsync(cancellationToken);
		if (rooms is null || rooms.Count == 0)
			return Result.Failure<IEnumerable<RoomResponse>>(RoomErrors.NoRoomsFound);

		var response = rooms.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms
			.Where(cr => cr.Id == roomId)
			.Include(cr => cr.ChatRoomUsers)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);

		if (room == null)
			return Result.Failure<RoomResponse>(RoomErrors.NoRoomsFound);

		var response = room.MapToRoomResponse();
		return Result.Success(response);
	}
	public async Task<Result<IEnumerable<RoomResponse>>> GetUserRoomsAsync(string userId, CancellationToken cancellationToken = default)
	{
		var userRooms = await _context.ChatRoomUsers
			.Where(cru => cru.UserId == userId)
			.Include(cru => cru.ChatRoom)
			.ThenInclude(cr => cr.ChatRoomUsers)
			.Select(cru => cru.ChatRoom)
			.ToListAsync(cancellationToken);

		if (userRooms == null || userRooms.Count == 0)
			return Result.Failure<IEnumerable<RoomResponse>>(RoomErrors.UserHasNoRooms);

		var response = userRooms.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result<RoomResponse>> CreateRoomAsync(FormRoomRequest request, CancellationToken cancellationToken = default)
	{
		var room = request.MapToChatRoom();
		var IsRoomExist = await _context.ChatRooms
			.AnyAsync(cr => cr.Name.ToLower() == request.Name.ToLower(), cancellationToken);
		if (IsRoomExist)
			return Result.Failure<RoomResponse>(RoomErrors.RoomAlreadyExists);

		await _context.ChatRooms.AddAsync(room, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		var roomUser = new ChatRoomUser
		{
			ChatRoomId = room.Id,
			UserId = request.CreatorUserId,
			IsAdmin = true,
			JoinedAt = DateTime.UtcNow
		};
		await _context.ChatRoomUsers.AddAsync(roomUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		var response = room.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result<RoomResponse>> UpdateRoomAsync(int roomId, FormRoomRequest request, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId);
		if (room == null)
			return Result.Failure<RoomResponse>(RoomErrors.NoRoomsFound);

		if (room.Name == request.Name && roomId != request.Id)
			return Result.Failure<RoomResponse>(RoomErrors.RoomAlreadyExists);

		var updatedRoom = request.MapToChatRoom(room);
		await _context.SaveChangesAsync(cancellationToken);

		var response = room.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result> JoinRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId, cancellationToken);
		if (room == null)
			return Result.Failure(RoomErrors.NoRoomsFound);

		var user = await _context.Users.FindAsync(userId, cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);

		var chatRoomUser = new ChatRoomUser()
		{
			UserId = userId,
			ChatRoomId = room.Id,
			JoinedAt = DateTime.UtcNow
		};
		await _context.ChatRoomUsers.AddAsync(chatRoomUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Groups.AddToGroupAsync(userId, $"Room_{roomId}", cancellationToken);
		await _hubContext.Clients.All.SendAsync("UserJoinedRoom", roomId, new
		{
			RoomId = roomId,
			RoomName = room.Name,
			UserName = $"{user.FirstName} {user.LastName}",
			UserId = userId
		}, cancellationToken: cancellationToken);
		return Result.Success();
	}

	public async Task<Result> LeaveRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId, cancellationToken);
		if (room == null)
			return Result.Failure(RoomErrors.NoRoomsFound);

		var user = await _context.Users.FindAsync(userId, cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);


		var chatRoomUser = await _context.ChatRoomUsers
			.FirstOrDefaultAsync(cru => cru.UserId == userId && cru.ChatRoomId == roomId, cancellationToken);
		if (chatRoomUser == null)
			return Result.Failure(UserError.UserNotFoundInRoom);

		_context.ChatRoomUsers.Remove(chatRoomUser);
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Groups.RemoveFromGroupAsync(userId, $"Room_{roomId}", cancellationToken);
		await _hubContext.Clients.All.SendAsync("UserLeftRoom", roomId, new
		{
			RoomId = roomId,
			RoomName = room.Name,
			UserName = $"{user.FirstName} {user.LastName}",
			UserId = userId
		}, cancellationToken: cancellationToken);
		return Result.Success();
	}

	public async Task<Result<PaginationList<MessageResponse>>> GetRoomMessagesAsync(int roomId, int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var messages= await _context.Messages
			.Where(m => m.ChatRoomId == roomId)
			.Include(m => m.Sender)
			.Include(m => m.Attachments)
			.OrderBy(m=>m.SentAt)
			.ToListAsync(cancellationToken);

		if (messages == null || messages.Count==0)
			return Result.Failure<PaginationList<MessageResponse>>(MessageErrors.MessageNotFound);

		var response = messages.MapToMessageResponse();
		var roomMessageResponse = new RoomMessageResponse
		(
			 response
		);
		var messageList = await PaginationList<MessageResponse>.CreateAsync(
			response,
			page,
			pageSize 
		);

		return Result.Success(messageList);
	}

	public async Task<Result> UserTypingAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var user = await _context.Users
			.Where(u => u.Id == userId)
			.Select(u => new { u.FirstName, u.LastName, u.Avatar })
			.FirstOrDefaultAsync(cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFoundInRoom);

		await _hubContext.Clients.GroupExcept(userId,$"Room_{roomId}").SendAsync("UserIsTyping", new
		{
			UserName = $"{user.FirstName} {user.LastName}",
			UserId= userId, 
			RoomId = roomId
		}, cancellationToken: cancellationToken);

		return Result.Success();
	}

	public async Task<Result> AddUserToRoomAsync(string userId, int roomId,CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId, cancellationToken);
		if (room == null)
			return Result.Failure(RoomErrors.NoRoomsFound);

		var user = await _context.Users.FindAsync(userId, cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);

		var chatRoomUser = new ChatRoomUser()
		{
			UserId = userId,
			ChatRoomId = room.Id,
			JoinedAt = DateTime.UtcNow
		};
		await _context.ChatRoomUsers.AddAsync(chatRoomUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Groups.AddToGroupAsync(userId, $"Room_{roomId}", cancellationToken);
		await _hubContext.Clients.All.SendAsync("UserAddedToRoom", roomId, new
		{
			RoomId = roomId,
			RoomName = room.Name,
			UserName = $"{user.FirstName} {user.LastName}",
			UserId = userId
		}, cancellationToken: cancellationToken);
		return Result.Success();
	}

	public async Task<Result> RemoveUserFromRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync( roomId, cancellationToken);
		if (room == null)
			return Result.Failure(RoomErrors.NoRoomsFound);

		var user = await _context.Users.FindAsync(userId, cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);


		var chatRoomUser = await _context.ChatRoomUsers
			.FirstOrDefaultAsync(cru => cru.UserId == userId && cru.ChatRoomId == roomId, cancellationToken);
		if (chatRoomUser == null)
			return Result.Failure(UserError.UserNotFoundInRoom);

		_context.ChatRoomUsers.Remove(chatRoomUser);
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Groups.RemoveFromGroupAsync(userId, $"Room_{roomId}", cancellationToken);
		await _hubContext.Clients.All.SendAsync("UserRemovedFromRoom", roomId, new
		{
			RoomId = roomId,
			RoomName = room.Name,
			UserName = $"{user.FirstName} {user.LastName}",
			UserId = userId
		}, cancellationToken: cancellationToken);
		return Result.Success();
	}

	public async Task<Result<IEnumerable<MessageResponse>>> GetUnreadMessagesAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ChatRoomId==roomId &&!m.IsRead)
			.OrderBy(m => m.SentAt)
			.Include(m => m.Sender)
			.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesUnread);

		var response = messages.MapToMessageResponse();
		return Result.Success(response);
	}
	public async Task<Result<int>> GetUnreadMessagesCountAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ChatRoomId==roomId && !m.IsRead)
			.CountAsync(cancellationToken);

		return Result.Success(messages);
	}

	public async Task<Result> MarkMessageAsReadAsync(int messageId, int roomId, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages
			.FirstOrDefaultAsync(m => m.Id == messageId && m.ChatRoomId==roomId, cancellationToken);

		if (message is null)
			return Result.Failure(MessageErrors.NoMessagesUnread);

		message.IsRead = true;

		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();

	}

	public async Task<Result> MarkAllMessageAsReadAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ChatRoomId==roomId && !m.IsRead)
			.ToListAsync(cancellationToken);
		if (messages is null || messages.Count == 0)
			return Result.Failure(MessageErrors.NoMessagesUnread);

		foreach (var message in messages)
			message.IsRead = true;

		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();

	}

	public async Task<Result> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId,cancellationToken);
		if (room == null)
			return Result.Failure(RoomErrors.NoRoomsFound);

		_context.ChatRooms.Remove(room);
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}


}

