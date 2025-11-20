using ChatApplication.API.DTOs;

namespace ChatApplication.API.Services.MessagesService;

public interface IMessagesService
{
	Task<Result<IEnumerable<MessageResponse>>> GetPrivateMessagesAsync(string userId1, string userId2,CancellationToken cancellationToken=default);

	Task<Result<IEnumerable<MessageResponse>>> GetRoomMessagesAsync(int roomId, int limit, CancellationToken cancellationToken=default);

	Task<Result<IEnumerable<MessageResponse>>> GetUnreadMessagesAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<int>> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
}
