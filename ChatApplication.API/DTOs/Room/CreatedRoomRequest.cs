namespace ChatApplication.API.DTOs.Room;

public record CreatedRoomRequest
(
	string Name,
	string Description,
	bool IsPrivate,
	DateTime CreatedAt,
	string CreatorUserId
);

