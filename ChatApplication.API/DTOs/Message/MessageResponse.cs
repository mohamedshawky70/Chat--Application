namespace ChatApplication.API.DTOs.Message;

public record MessageResponse
(
	string Content,
	string SenderId,
	string? ReseiverId,
	DateTime SentAt,
	bool IsRead,
	string Type,
	string SenderName,
	int? ChatRoomId
);
