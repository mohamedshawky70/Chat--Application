using ChatApplication.API.DTOs.Account;

namespace ChatApplication.API.Services.AccountService;

public interface IAccountService
{
	Task<Result<UserProfileResponse>> UserProfileAsync(string userId, CancellationToken canellationToken = default);

	Task<Result<UserProfileRequest>> UpdateUserProfileAsync(string userId, UserProfileRequest request, CancellationToken canellationToken = default);

	Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken canellationToken = default);
}
