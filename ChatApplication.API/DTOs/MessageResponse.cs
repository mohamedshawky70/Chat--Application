using ChatApplication.API.enums;

namespace ChatApplication.API.DTOs;

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
