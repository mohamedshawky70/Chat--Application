namespace ChatApplication.API.DTOs.Message;

public record MessageResponse
(
	string Content,
	string SenderId,
	string? ReseiverId,
	DateTime SentAt,
	bool IsRead,
	bool IsPinned,
	string PinnedBy,
	string Type,
	string SenderName,
	int? ChatRoomId
);
