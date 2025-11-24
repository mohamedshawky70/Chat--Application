using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserServeic userService) : ControllerBase
{
	private readonly IUserServeic _userService = userService;

	[HttpGet("online")]
	public async Task<IActionResult> GetOnlineUsers(CancellationToken cancellationToken)
	{
		var result = await _userService.GetOnlineUsersAsync(cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("online-in-room/{roomId}")]
	public async Task<IActionResult> GetOnlineUsersInRoom([FromRoute] int roomId,  CancellationToken cancellationToken)
	{
		var result = await _userService.GetOnlineUsersInRoomAsync(roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("search")]
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

	[HttpPut("set-as-admin/{userId}/{roomId}")]
	public async Task<IActionResult> SetAsAdmin([FromRoute] string userId, [FromRoute] int roomId, CancellationToken cancellationToken)
	{
		var result = await _userService.SetAsAdminAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}
	
	[HttpPut("remove-from-admin/{userId}/{roomId}")]
	public async Task<IActionResult> RemoveFromAdmin([FromRoute] string userId, [FromRoute] int roomId, CancellationToken cancellationToken)
	{
		var result = await _userService.RemoveFromAdminAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("last-seen/{userId}")]
	public async Task<IActionResult> GetLastSeen([FromRoute] string userId, CancellationToken cancellationToken)
	{
		var result = await _userService.GetLastSeenAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPost("block-user/{blockerId}/{blockedId}")]
	public async Task<IActionResult> BlockUser([FromRoute] string blockerId, [FromRoute] string blockedId, CancellationToken cancellationToken)
	{
		var result = await _userService.BlockUserAsync(blockerId, blockedId, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}
}
