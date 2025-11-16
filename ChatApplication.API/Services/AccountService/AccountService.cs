using ChatApplication.API.DTOs.User;
using ChatApplication.API.Mapping;
using Microsoft.AspNetCore.Identity;

namespace ChatApplication.API.Services.AccountService;

public class AccountService(ApplicationDbContext context, UserManager<User> userManager) : IAccountService
{
	private readonly ApplicationDbContext _context = context;
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

		var response = user.MapToResponse();
		return Result.Success(response);
	}
}
