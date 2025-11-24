using ChatApplication.API.DTOs.Message;

namespace ChatApplication.API.DTOs.Room;

public record RoomMessageResponse
(
	IEnumerable<MessageResponse> Messages
);

