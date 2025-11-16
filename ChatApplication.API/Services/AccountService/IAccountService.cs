using ChatApplication.API.DTOs.User;

namespace ChatApplication.API.Services.AccountService;

public interface IAccountService
{
	Task<Result<UserProfileResponse>> UserProfileAsync(string userId, CancellationToken canellationToken = default);
}
