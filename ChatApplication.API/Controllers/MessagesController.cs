using ChatApplication.API.DTOs.Message;
using ChatApplication.API.Services.MessagesService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController(IMessagesService messagesService) : ControllerBase
{
	private readonly IMessagesService _messagesService = messagesService;

	[HttpGet("private/{userId1}/{userId2}")]
	public async Task<IActionResult> GetPrivateMessages([FromRoute] string userId1, [FromRoute] string userId2, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.GetPrivateMessagesAsync(userId1, userId2, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("room/{roomId}")]                                                      //set limit in params section
	public async Task<IActionResult> GetRoomMessages([FromRoute] int roomId, [FromQuery] int limit = 50, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.GetRoomMessagesAsync(roomId, limit, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("unread/{userId}")]
	public async Task<IActionResult> GetUnreadMessages([FromRoute] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.GetUnreadMessagesAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("unread_count/{userId}")]
	public async Task<IActionResult> GetUnreadCount([FromRoute] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.GetUnreadMessagesCountAsync(userId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPut("mark_read/{messageId}")]
	public async Task<IActionResult> MarkMessageAsRead([FromRoute] int messageId, [FromQuery] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.MarkMessageAsReadAsync(messageId, userId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}

	[HttpPut("mark_all_read/{userId}")]
	public async Task<IActionResult> MarkAllMessagesAsRead([FromRoute] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.MarkAllMessageAsReadAsync(userId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}

	[HttpPut("Edit/{messageId}")]
	public async Task<IActionResult> EditMessage([FromRoute] int messageId, [FromBody] EditMessageRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.EditMessageAsync(messageId, request, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}
	[HttpPut("delete/{messageId}")] //Soft delete
	public async Task<IActionResult> DeleteMessage([FromRoute] int messageId,[FromQuery] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.DeleteMessageAsync(messageId,userId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}
}