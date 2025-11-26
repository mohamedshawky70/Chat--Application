using ChatApplication.API.DTOs.Room;
using ChatApplication.API.Entites;
using ChatApplication.API.Services.RoomService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
	public async Task<IActionResult> CreateRoom([FromBody] FormRoomRequest request, CancellationToken cancellationToken)
	{
		var result = await _roomService.CreateRoomAsync(request, cancellationToken);
		if (result.IsSuccess)                                                              // created object
			return CreatedAtAction(nameof(GetAllRooms), new { userId = result.Value().Id }, result.Value());

		else
			return Conflict(result.Error);

	}

	[HttpPut("{roomId}")]
	public async Task<IActionResult> UpdateRoom([FromRoute] int roomId, [FromBody] FormRoomRequest request, CancellationToken cancellationToken)
	{
		var result = await _roomService.UpdateRoomAsync(roomId, request, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPost("join")]
	public async Task<IActionResult> JoinRoom([FromQuery] string userId, [FromQuery] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.JoinRoomAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}

	[HttpPost("leave")]
	public async Task<IActionResult> LeaveRoom([FromQuery] string userId, [FromQuery] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.LeaveRoomAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}
	
	[HttpPost("add")]
	public async Task<IActionResult> AddToRoom([FromQuery] string userId, [FromQuery] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.AddUserToRoomAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}

	[HttpPost("remove")]
	public async Task<IActionResult> RemoveUserFromRoom([FromQuery] string userId, [FromQuery] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.RemoveUserFromRoomAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}

	[HttpGet("room-messages/{roomId}")]
	public async Task<IActionResult> GetRoomMessages([FromRoute] int roomId,CancellationToken cancellationToken, int page = 1, int pageSize = 50)
	{
		var result = await _roomService.GetRoomMessagesAsync(roomId,page ,pageSize ,  cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("user-typing/{userId}")]
	public async Task<IActionResult> UserTyping([FromRoute] string userId, int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.UserTypingAsync(userId, roomId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}

	[HttpGet("unread/{rooId}")]
	public async Task<IActionResult> GetUnreadMessages([FromRoute] int rooId, CancellationToken cancellationToken = default)
	{
		var result = await _roomService.GetUnreadMessagesAsync(rooId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("unread_count/{roomId}")]
	public async Task<IActionResult> GetUnreadCount([FromRoute] int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _roomService.GetUnreadMessagesCountAsync(roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	
	[HttpPut("mark_read/{messageId}")]
	public async Task<IActionResult> MarkMessageAsRead([FromRoute] int messageId, [FromQuery] int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _roomService.MarkMessageAsReadAsync(messageId, roomId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}

	[HttpPut("mark_all_read/{roomId}")]
	public async Task<IActionResult> MarkAllMessagesAsRead([FromRoute] int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _roomService.MarkAllMessageAsReadAsync(roomId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}

	[HttpDelete("{roomId}")]
	public async Task<IActionResult> DeleteRoom([FromRoute] int roomId, CancellationToken cancellationToken)
	{
		var result = await _roomService.DeleteRoomAsync(roomId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}
}
