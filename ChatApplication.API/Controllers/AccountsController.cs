using ChatApplication.API.DTOs.Account;
using ChatApplication.API.DTOs.File;
using ChatApplication.API.Services.AccountService;
using ChatApplication.API.Services.FileService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]

//[Authorize]
public class AccountsController(IAccountService accountService,IFileService fileService) : ControllerBase
{
	private readonly IAccountService _accountService = accountService;
	private readonly IFileService _fileService = fileService;

	[HttpPost("upload-profile-avatar")]
	public async Task<IActionResult> UploadProfileAvatar([FromForm] UploadProfileAvatarRequest request, CancellationToken cancellationToken)
	{
		var result = await _fileService.UploadProfileAvatarAsync(request, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}

	[HttpGet("profile")]
	public async Task<IActionResult> UserProfile(CancellationToken cancellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.UserProfileAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result);
	}
	
	[HttpGet("other-profile/{userId}")]
	public async Task<IActionResult> OtherUserProfile([FromRoute] string userId,CancellationToken cancellationToken)
	{
		var result = await _accountService.OtherUserProfileAsync(userId, cancellationToken);
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

	[HttpPut("activate")]
	public async Task<IActionResult> ActivateAccount(CancellationToken canellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.ActivateAsync(userId, canellationToken);
		return result.IsSuccess ? NoContent() : BadRequest(result);
	}
	
	[HttpPut("deactivate")]
	public async Task<IActionResult> DeactivateAccount(CancellationToken canellationToken)
	{
		throw new Exception("My EX");
		var userId = User.GetUserId();
		var result = await _accountService.DeactivateAsync(userId, canellationToken);
		return result.IsSuccess ? NoContent() : BadRequest(result);
	}

	[HttpDelete("account")]
	public async Task<IActionResult> DeleteAccount(CancellationToken canellationToken)
	{
		var userId = User.GetUserId();
		var result = await _accountService.DeleteAsync(userId,canellationToken);
		return result.IsSuccess ? NoContent() : BadRequest(result);
	}
}
