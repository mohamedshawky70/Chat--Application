using ChatApplication.API.DTOs.User;

namespace ChatApplication.API.Services.UserService;

public interface IUserServeic
{
	Task<Result<IEnumerable<UserResponse>>> GetAllAsync(FilterRequest request, CancellationToken cancellationToken = default);

	Task<Result<UserResponse>> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersAsync(CancellationToken cancellationToken = default);
}
