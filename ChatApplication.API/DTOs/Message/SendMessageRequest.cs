
namespace ChatApplication.API.DTOs.Message;

public record SendMessageRequest
(
	string? Content,
    MessageType Type = MessageType.Text,
    string? ReceiverId = null,
    int? ChatRoomId = null,
    List<IFormFile>? Files = null
);
