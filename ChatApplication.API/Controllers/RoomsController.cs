using ChatApplication.API.DTOs.Room;
using ChatApplication.API.Services.RoomService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController(IRoomService roomService) : ControllerBase
{
	private readonly IRoomService _roomService = roomService;

	[HttpGet]
	public async Task<IActionResult> GetAllRooms(CancellationToken cancellationToken)
	{
		var result = await _roomService.GetAllRoomsAsync(cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("{roomId}")]
	public async Task<IActionResult> GetRoomById([FromRoute] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.GetRoomByIdAsync(roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("userRooms/{userId}")]
	public async Task<IActionResult> GetUserRooms(string userId, CancellationToken cancellationToken)
	{
		var result = await _roomService.GetUserRoomsAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPost("")]
	public async Task<IActionResult> CreateRoom([FromBody] CreatedRoomRequest request, CancellationToken cancellationToken)
	{
		var result = await _roomService.CreateRoomAsync(request, cancellationToken);
		if (result.IsSuccess)                                                              // created object
			return CreatedAtAction(nameof(GetAllRooms), new { userId = result.Value().Id }, result.Value());

		else
			return Conflict(result.Error);

	}
}
