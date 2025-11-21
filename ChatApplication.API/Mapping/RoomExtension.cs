using ChatApplication.API.DTOs.Room;

namespace ChatApplication.API.Mapping;

public static class RoomExtension
{
	public static ChatRoom MapToChatRoom(this CreatedRoomRequest request)
	{
		return new ChatRoom
		{
			Name = request.Name,
			Description = request.Description,
			IsPrivate = request.IsPrivate,
			CreatedAt = request.CreatedAt,
			CreatorUserId = request.CreatorUserId
		};
	}
	public static RoomResponse MapToRoomResponse(this ChatRoom room)
	{
		return new RoomResponse
			(
				room.Id,
				room.Name,
				room.Description,
				room.CreatedAt,
				room.ChatRoomUsers.Count,
				room.IsPrivate
			);
		
		
	}
	public static IEnumerable<RoomResponse> MapToRoomResponse(this IEnumerable<ChatRoom> rooms)
	{
		return rooms.Select(r=>r.MapToRoomResponse());
		
	}
}
