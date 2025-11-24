namespace ChatApplication.API.DTOs.Room;

public record FormRoomRequest
(
	int Id,
	string Name,
	string Description,
	bool IsPrivate,
	DateTime CreatedAt,
	string CreatorUserId
);

