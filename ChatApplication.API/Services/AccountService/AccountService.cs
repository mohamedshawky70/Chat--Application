using ChatApplication.API.DTOs.Account;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.AccountService;

public class AccountService(UserManager<User> userManager) : IAccountService
{
	private readonly UserManager<User> _userManager = userManager;

	public async Task<Result<UserProfileResponse>> UserProfileAsync(string userId, CancellationToken canellationToken = default)
	{

		//var user = await _context.Users
		//	.AsNoTracking()
		//	.FirstOrDefaultAsync(u => u.Id == userId, canellationToken);

		//The best because optimized, uses index, already includes AsNoTracking-like behavior
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<UserProfileResponse>(UserError.UserNotFound);

		var response = user.MapToUserProfileResponse();
		return Result.Success(response);
	}
	public async Task<Result<UserProfileRequest>> UpdateUserProfileAsync(string userId, UserProfileRequest request, CancellationToken canellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<UserProfileRequest>(UserError.UserNotFound);

		var updatedUser = request.MapToUser(user);

		var result = await _userManager.UpdateAsync(updatedUser);
		if (result.Succeeded)
			return Result.Success(request);

		return Result.Failure<UserProfileRequest>(UserError.UpdateFailed);
	}

	public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken canellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);
		var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword); //already handeled (currentReqPass ==  currentDbPass)
		if (result.Succeeded)
			return Result.Success();
		return Result.Failure(UserError.ChangePasswordFailed);
	}
}
