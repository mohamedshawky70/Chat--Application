using ChatApplication.API.DTOs;
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
	public async Task<Result<int>> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
	{
		var messages = await _context.Messages
			.Where(m => m.ReceiverId == userId && !m.IsRead)
			.CountAsync(cancellationToken);

		return Result.Success(messages);
	}
}
