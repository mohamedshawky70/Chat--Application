using ChatApplication.API.DTOs.Account;
using ChatApplication.API.Services.AccountService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]

[Authorize]
public class AccountsController(IAccountService accountService) : ControllerBase
{
	private readonly IAccountService _accountService = accountService;
	[HttpGet("profile")]
	public async Task<IActionResult> UserProfile(CancellationToken cancellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.UserProfileAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result);
	}

	[HttpPut("profile")]
	public async Task<IActionResult> UpdateUserProfile(UserProfileRequest request, CancellationToken canellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.UpdateUserProfileAsync(userId, request, canellationToken);
		return result.IsSuccess ? Ok(result.Value()) : BadRequest(result);
	}

	[HttpPut("change-password")]
	public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken canellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.ChangePasswordAsync(userId, request, canellationToken);
		return result.IsSuccess ? Ok() : BadRequest(result);
	}
}
