namespace ChatApplication.API.Services.UserService;

public interface IUserServeic
{
	Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<UserResponse>>> SearchAsync(FilterRequest request, CancellationToken cancellationToken = default);

	Task<Result<UserResponse>> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersAsync(CancellationToken cancellationToken = default);

	Task<Result<UserResponse>> SetAsAdminAsync(string userId, int roomId, CancellationToken cancellationToken = default);

	Task<Result<UserResponse>> RemoveFromAdminAsync(string userId, int roomId, CancellationToken cancellationToken = default);

	Task<Result<bool>> IsAdminInRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersInRoomAsync(int roomId, CancellationToken cancellationToken = default);

	Task<Result<DateTime>> GetLastSeenAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result> BlockUserAsync(string blockerId, string blockedId, CancellationToken cancellationToken = default);
	
	Task<Result> UnBlockUserAsync(string blockerId, string blockedId, CancellationToken cancellationToken = default);

	Task<Result<bool>> IsBlockedAsync(string userId1, string userId2, CancellationToken cancellationToken = default);
}
