namespace ChatApplication.API.DTOs.Room;

public record RoomResponse
(
	int Id,
	string Name,
	string Description,
	DateTime CreatedAt,
	int MemberCount,
	bool IsPrivate
);
