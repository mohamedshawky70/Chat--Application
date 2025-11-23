using ChatApplication.API.DTOs.Message;
using ChatApplication.API.Services.MessagesService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController(IMessagesService messagesService) : ControllerBase
{
	private readonly IMessagesService _messagesService = messagesService;

	//If you don't wan't real-time
	[HttpPost("private")]
	//With Attachment
	public async Task<IActionResult> SendPrivateMessage([FromForm] SendMessageRequest request , CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.SendPrivateMessageAsync(request.SenderId!,request.ReceiverId!,request.Content!,request.File, cancellationToken);
		return result.IsSuccess ? CreatedAtAction(nameof(GetPrivateMessages), new { userId1 = request.SenderId, userId2 = request.ReceiverId }, result.Value()) : NotFound(result.Error);
	}
	
	[HttpPost("room")]
	public async Task<IActionResult> SendRoomMessage([FromForm] SendMessageRequest request , CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.SendRoomMessageAsync(request.SenderId!,request.ChatRoomId!,request.Content!,request.File, cancellationToken);
		return result.IsSuccess ? CreatedAtAction(nameof(GetRoomMessages), new { roomId = request.ChatRoomId}, result.Value()) : NotFound(result.Error);
	}

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

	[HttpGet("search/{userId1}/{userId2}")]
	public async Task<IActionResult> SearchMessages([FromRoute] string userId1, [FromRoute] string userId2, [FromBody] FilterRequest filter, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.MessageSearchAsync(userId1, userId2, filter, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("room_search/{roomId}")]
	public async Task<IActionResult> SearchRoomMessages([FromRoute] int roomId, [FromBody] FilterRequest filter, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.SearchRoomMessagesAsync(roomId, filter, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPost("forward/{messageId}")]
	public async Task<IActionResult> ForwardMessage([FromRoute] int messageId, [FromBody] ForwardMessageRequest request,  CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.ForwardMessageAsync(messageId, request, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpPut("pin/{messageId}")]
	public async Task<IActionResult> PinMessage([FromRoute] int messageId,[FromQuery]string userId, [FromQuery]int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.PinnedMessageAsync(messageId,userId,roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("pinned")]
	public async Task<IActionResult> GetPinnedMessages([FromQuery]string userId, [FromQuery]int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.GetpinnedMessagesAsync(userId,roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}


	[HttpPut("unpin/{messageId}")]
	public async Task<IActionResult> UnpinMessage([FromRoute] int messageId,[FromQuery]string userId, [FromQuery]int roomId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.UnpinnedMessageAsync(messageId,userId,roomId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : NotFound(result.Error);
	}

	[HttpGet("download/{id}")]
	public async Task<IActionResult> DownloadAttachment([FromRoute] Guid id, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.DownloadFileAsync(id, cancellationToken);
		return result.IsSuccess ? File(result.Value().fileContent, result.Value().ContentType, result.Value().fileName) : NotFound(result.Error);
	}

	[HttpPut("delete/{messageId}")] //Soft delete
	public async Task<IActionResult> DeleteMessage([FromRoute] int messageId,[FromQuery] string userId, CancellationToken cancellationToken = default)
	{
		var result = await _messagesService.DeleteMessageAsync(messageId,userId, cancellationToken);
		return result.IsSuccess ? NoContent() : NotFound(result.Error);
	}
}