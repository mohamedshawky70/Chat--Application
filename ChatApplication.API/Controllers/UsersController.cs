using ChatApplication.API.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserServeic userService) : ControllerBase
{
	private readonly IUserServeic _userService = userService;

	[HttpGet("")]
	public async Task<IActionResult> GetAll([FromBody] FilterRequest request, CancellationToken cancellationToken)
	{
		var result = await _userService.GetAllAsync(request, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : BadRequest(result.Error);
	}

	[HttpGet("{userId}")]
	public async Task<IActionResult> GetById([FromRoute] string userId, CancellationToken cancellationToken)
	{
		var result = await _userService.GetByIdAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("online")]
	public async Task<IActionResult> GetOnlineUsers(CancellationToken cancellationToken)
	{
		var result = await _userService.GetOnlineUsersAsync(cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}
}
