using ChatApplication.API.DTOs.File;
using ChatApplication.API.DTOs.Message;
using ChatApplication.API.Hubs;
using ChatApplication.API.Mapping;
using ChatApplication.API.Services.FileService;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.API.Services.MessagesService;

public class MessagesService(ApplicationDbContext context, IHubContext<ChatHub> hubContext, IFileService fileService) : IMessagesService
{
	private readonly ApplicationDbContext _context = context;
	private readonly IHubContext<ChatHub> _hubContext = hubContext;
	private readonly IFileService _fileService = fileService;

	public async Task<Result<MessageResponse>> SendPrivateMessageAsync(string senderId, string recieverId, string content, IFormFile? file, CancellationToken cancellationToken = default)
	{
		var reciever = await _context.Users.FindAsync(recieverId);
		if (reciever is null)
			return Result.Failure<MessageResponse>(MessageErrors.UserNotFound);

		var type = MessageType.Text;

		var message = new Message()
		{
			SenderId = senderId,
			Content = content,
			ReceiverId = recieverId,
			SentAt = DateTime.UtcNow,
			IsRead = false,
			Type = type
		};

		await _context.Messages.AddAsync(message, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		if (file != null)
			await ContentType(file, message);

		var response = message.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<MessageResponse>> SendRoomMessageAsync(string senderId, int roomId, string content, IFormFile? file, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms.FindAsync(roomId);
		if (room is null)
			return Result.Failure<MessageResponse>(RoomErrors.NoRoomsFound);

		var type = MessageType.Text;

		var message = new Message()
		{
			SenderId = senderId,
			Content = content,
			ChatRoomId = roomId,
			SentAt = DateTime.UtcNow,
			IsRead = false,
		};

		await _context.Messages.AddAsync(message, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		if (file != null)
			await ContentType(file, message);

		var response = message.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<MessageResponse>>> GetPrivateMessagesAsync(string userId1, string userId2, CancellationToken cancellationToken = default)
	{

		var messages = await _context.Messages
					.Where(m => m.SenderId == userId1 && m.ReceiverId == userId2 ||
						  (m.SenderId == userId2 && m.ReceiverId == userId1))
					.OrderBy(m => m.SentAt)
					.Include(m => m.Sender)
					.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesFound);

		var response = messages.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<MessageResponse>>> GetRoomMessagesAsync(int roomId, int limit, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ChatRoomId == roomId)
			.OrderByDescending(m => m.SentAt)
			.Take(limit)
			.Include(m => m.Sender)
			.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesFound);

		messages.Reverse(); // Display Oldest messages first
		var response = messages.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<MessageResponse>>> GetUnreadMessagesAsync(string userId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ReceiverId == userId && !m.IsRead)
			.OrderBy(m => m.SentAt)
			.Include(m => m.Sender)
			.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoMessagesUnread);

		var response = messages.MapToMessageResponse();
		return Result.Success(response);
	}
	public async Task<Result<int>> GetUnreadMessagesCountAsync(string userId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ReceiverId == userId && !m.IsRead)
			.CountAsync(cancellationToken);

		return Result.Success(messages);
	}

	public async Task<Result> MarkMessageAsReadAsync(int messageId, string userId, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages
			.FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId, cancellationToken);

		if (message is null)
			return Result.Failure(MessageErrors.NoMessagesUnread);

		message.IsRead = true;

		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();

	}

	public async Task<Result> MarkAllMessageAsReadAsync(string userId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ReceiverId == userId && !m.IsRead)
			.ToListAsync(cancellationToken);
		if (messages is null || messages.Count == 0)
			return Result.Failure(MessageErrors.NoMessagesUnread);

		foreach (var message in messages)
			message.IsRead = true;

		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();

	}

	public async Task<Result<MessageResponse>> EditMessageAsync(int messageId, EditMessageRequest request, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages.FindAsync(messageId);
		if (message is null)
			return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

		message.Content = request.NewContent;
		await _context.SaveChangesAsync(cancellationToken);

		var response = message.MapToMessageResponse();
		return Result.Success(response);
	}

	//In Private messages between two users
	public async Task<Result<IEnumerable<MessageResponse>>> MessageSearchAsync(string userId1, string userId2, FilterRequest filter, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
						(m.SenderId == userId2 && m.ReceiverId == userId1))
			.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.MessageNotFound);

		var SearchValueTrim = filter.SearchValue.Trim().ToLower();
		var filteredMessages = messages
			.Where(m => m.Content.ToLower()
			.Contains(SearchValueTrim))
			.ToList();

		if (filteredMessages is null || filteredMessages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.MessageNotFound);

		var response = filteredMessages.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<MessageResponse>>> SearchRoomMessagesAsync(int roomId1, FilterRequest filter, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ChatRoomId == roomId1)
			.ToListAsync(cancellationToken);

		if (messages is null || messages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.MessageNotFound);

		var SearchValueTrim = filter.SearchValue.Trim().ToLower();
		var filteredMessages = messages
			.Where(m => m.Content.ToLower()
			.Contains(SearchValueTrim))
			.ToList();

		if (filteredMessages is null || filteredMessages.Count == 0)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.MessageNotFound);

		var response = filteredMessages.MapToMessageResponse();
		return Result.Success(response);

	}

	public async Task<Result<IEnumerable<MessageResponse>>> ForwardMessageAsync(int messageId, ForwardMessageRequest request, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages
			.AsNoTracking()
			.Include(m => m.Sender)
			.FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

		if (message is null)
			return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.MessageNotFound);

		IEnumerable<MessageResponse> response = null!;
		var forwardedMessages = new List<Message>();

		// Forward to Users
		if (request.ForwardToUserIds != null && request.ForwardToUserIds.Count > 0)
		{
			foreach (var ReceiverId in request.ForwardToUserIds)
			{
				if (ReceiverId == message.SenderId)//don't forward to self
					continue;

				var newMessage = new Message
				{
					SentAt = DateTime.UtcNow,
					IsRead = false,
					Type = message.Type,
					SenderId = message.SenderId, // Return thinking
					Content = string.IsNullOrEmpty(request.Caption) ? $"Forwarded message: {message.Content}"
															: $"{request.Caption} Forwarded: {message.Content}",
					ReceiverId = ReceiverId
				};
				forwardedMessages.Add(newMessage);
			}
		}

		// Forward to Rooms
		if (request.ForwardToRoomIds != null && request.ForwardToRoomIds.Count > 0)
		{
			foreach (var roomId in request.ForwardToRoomIds)
			{
				var newMessage = new Message
				{
					SentAt = DateTime.UtcNow,
					IsRead = false,
					Type = message.Type,
					SenderId = message.SenderId, // Return thinking
					Content = string.IsNullOrEmpty(request.Caption) ? $"Forwarded message: {message.Content}"
															: $"{request.Caption} Forwarded: {message.Content}",
					ReceiverId = null,
					ChatRoomId = roomId
				};
				forwardedMessages.Add(newMessage);
			}
			await _context.AddRangeAsync(forwardedMessages, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

		}

		response = forwardedMessages.MapToMessageResponse();

		// Start real-time
		foreach (var fMessage in forwardedMessages)
		{
			if (fMessage.ReceiverId != null)
			{
				await _hubContext.Clients.User(fMessage.ReceiverId!).SendAsync("ReceiveMessage", fMessage.MapToMessageResponse(), cancellationToken);
			}
			else if (fMessage.ChatRoomId != null)
			{
				await _hubContext.Clients.Group($"ChatRoom-{fMessage.ChatRoomId}").SendAsync("ReceiveMessage", fMessage.MapToMessageResponse(), cancellationToken);
			}
		}
		// End real-time
		return Result.Success(response);
	}

	//Pin message in Private or Room
	public async Task<Result<MessageResponse>> PinnedMessageAsync(int messageId, string userId, int? roomId, CancellationToken cancellationToken = default)
	{
		MessageResponse response = null!;
		// Sender or receiver can pin the message in private chat
		var message = new Message();
		if (userId != null && roomId == 0)
		{
			message = await _context.Messages
		   .Include(m => m.Sender)
		   .Include(m => m.Receiver)
		   .FirstOrDefaultAsync(m => m.Id == messageId && (m.SenderId == userId ||
								m.ReceiverId == userId), cancellationToken);
			if (message is null)
				return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

			message.IsPinned = true;
			message.PinnedId = userId;
			await _context.SaveChangesAsync(cancellationToken);
		}
		else //pin in Room
		{
			message = await _context.Messages
			.Include(m => m.Sender)
			.Include(m => m.Receiver)
			.FirstOrDefaultAsync(m => m.Id == messageId && m.ChatRoomId == roomId && (m.SenderId == userId ||
								 m.ReceiverId == userId), cancellationToken);
			if (message is null)
				return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

			message.IsPinned = true;
			message.PinnedId = userId;
			await _context.SaveChangesAsync(cancellationToken);
		}
		response = message.MapToMessageResponse();

		//Start real-time 
		await _hubContext.Clients.Group(roomId != null ? $"ChatRoom-{roomId}" : $"PrivateChat-{userId}")
			.SendAsync("MessagePinned", response, cancellationToken);
		//End real-time
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<MessageResponse>>> GetpinnedMessagesAsync(string userId, int? roomId, CancellationToken cancellationToken = default)
	{
		var pinnedMessages = new List<Message>();
		if (userId != null && roomId == 0)
		{
			pinnedMessages = await _context.Messages
			.Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.IsPinned)
			.Include(m => m.Sender)
			.Include(m => m.Receiver)
			.ToListAsync(cancellationToken);
			if (pinnedMessages is null || pinnedMessages.Count == 0)
				return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoPinnedMessagesFound);
		}
		else
		{
			pinnedMessages = await _context.Messages
		   .Where(m => m.ChatRoomId == roomId && m.IsPinned)
		   .Include(m => m.Sender)
		   .Include(m => m.Receiver)
		   .ToListAsync(cancellationToken);
			if (pinnedMessages is null || pinnedMessages.Count == 0)
				return Result.Failure<IEnumerable<MessageResponse>>(MessageErrors.NoPinnedMessagesFound);
		}
		var response = pinnedMessages.MapToMessageResponse();
		return Result.Success(response);
	}

	public async Task<Result<MessageResponse>> UnpinnedMessageAsync(int messageId, string userId, int? roomId, CancellationToken cancellationToken = default)
	{
		MessageResponse response = null!;
		// Sender or receiver can unpin the message in private chat
		var message = new Message();
		if (userId != null && roomId == 0)
		{
			message = await _context.Messages
			.FirstOrDefaultAsync(m => m.Id == messageId && (m.SenderId == userId ||
								 m.ReceiverId == userId), cancellationToken);
			if (message is null)
				return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

			message.IsPinned = false;
			message.PinnedId = null;
			await _context.SaveChangesAsync(cancellationToken);
		}
		else //unpin in Room
		{
			message = await _context.Messages
		   .FirstOrDefaultAsync(m => m.Id == messageId && m.ChatRoomId == roomId && (m.SenderId == userId ||
								m.ReceiverId == userId), cancellationToken);
			if (message is null)
				return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

			message.IsPinned = false;
			message.PinnedId = null;
			await _context.SaveChangesAsync(cancellationToken);
		}
		response = message.MapToMessageResponse();

		//Start real-time 
		await _hubContext.Clients.Group(roomId != null ? $"ChatRoom-{roomId}" : $"PrivateChat-{userId}")
			.SendAsync("MessageUnpinned", response, cancellationToken);
		//End real-time
		return Result.Success(response);
	}

	public async Task<Result<(byte[] fileContent, string ContentType, string fileName)>> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var response = await _fileService.DownloadFileAsync(id, cancellationToken);
		return Result.Success(response);
	}

	public async Task<Result> UserTypingAsync(string userId1, string userId2, CancellationToken cancellationToken = default)
	{
		var user = await _context.Users
			.Where(u => u.Id == userId2)
			.Select(u => new { u.FirstName, u.LastName, u.Avatar })
			.FirstOrDefaultAsync(cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFoundInRoom);

		await _hubContext.Clients.Client($"User_{userId1}").SendAsync("UserIsTyping", new
		{
			UserName = $"{user.FirstName} {user.LastName}",
			UserId= userId2, 
		}, cancellationToken: cancellationToken);

		return Result.Success();
	}

	public async Task<Result> DeleteMessageAsync(int messageId, string userId, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages
			.FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == userId, cancellationToken);

		if (message is null)
			return Result.Failure(MessageErrors.MessageNotFound);

		message.IsDeleted = true;
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}

	private async Task ContentType(IFormFile file, Message message)
	{
		var type = MessageType.Text;
		if (file.ContentType.StartsWith("image/"))
			type = MessageType.Image;
		if (file.ContentType.StartsWith("application/") ||
			file.ContentType.StartsWith("video/") ||
			file.ContentType.StartsWith("audio/"))
			type = MessageType.File;

		message.Type = type;
		await _context.SaveChangesAsync();

		var uploadFileRquest = new UploadFileRquest(file);
		await _fileService.UploadFileAsync(uploadFileRquest, message.Id);
	}
}
