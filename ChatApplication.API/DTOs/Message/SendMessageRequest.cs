
namespace ChatApplication.API.DTOs.Message;

public record SendMessageRequest
(
	string? Content,
    int ChatRoomId,
    string? SenderId = null,
    string? ReceiverId = null,
    IFormFile? File = null
);
