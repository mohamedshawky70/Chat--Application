using ChatApplication.API.DTOs.Message;

namespace ChatApplication.API.Services.MessagesService;

public interface IMessagesService
{
	Task<Result<IEnumerable<MessageResponse>>> GetPrivateMessagesAsync(string userId1, string userId2, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> GetRoomMessagesAsync(int roomId, int limit, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> GetUnreadMessagesAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<int>> GetUnreadMessagesCountAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result> MarkAllMessageAsReadAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result> MarkMessageAsReadAsync(int messageId, string userId, CancellationToken cancellationToken = default);

	Task<Result<MessageResponse>> EditMessageAsync(int messageId, EditMessageRequest request, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> MessageSearchAsync(string userId1, string userId2, FilterRequest filter, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> SearchRoomMessagesAsync(int roomId1, FilterRequest filter, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> ForwardMessageAsync(int messageId, ForwardMessageRequest request, CancellationToken cancellationToken = default);

	Task<Result<MessageResponse>> PinnedMessageAsync(int messageId, string userId, int? roomId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> GetpinnedMessagesAsync(string userId, int? roomId, CancellationToken cancellationToken = default);
	
	Task<Result<MessageResponse>> UnpinnedMessageAsync(int messageId, string userId, int? roomId, CancellationToken cancellationToken = default);

	Task<Result> DeleteMessageAsync(int messageId, string userId, CancellationToken cancellationToken = default);
}
