using ChatApplication.API.DTOs.Message;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.MessagesService;

public class MessagesService(ApplicationDbContext context) : IMessagesService
{
	private readonly ApplicationDbContext _context = context;

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
	
	public async Task<Result> MarkMessageAsReadAsync(int messageId ,string userId, CancellationToken cancellationToken = default)
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

	public async Task<Result<MessageResponse>> EditMessageAsync(int messageId,EditMessageRequest request, CancellationToken cancellationToken = default)
	{
		var message = await _context.Messages.FindAsync(messageId);
		if (message is null)
			return Result.Failure<MessageResponse>(MessageErrors.MessageNotFound);

		message.Content=request.NewContent;
		await _context.SaveChangesAsync(cancellationToken);

		var response = message.MapToMessageResponse();
		return Result.Success(response);
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
}
