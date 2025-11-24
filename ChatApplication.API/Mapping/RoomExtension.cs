using ChatApplication.API.DTOs.Room;

namespace ChatApplication.API.Mapping;

public static class RoomExtension
{
	// Create
	public static ChatRoom MapToChatRoom(this FormRoomRequest request)
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

	// Update
	public static ChatRoom MapToChatRoom(this FormRoomRequest request, ChatRoom room)
	{
		var updatedRoom=room;
		
		room.Name = request.Name;
		room.Description = request.Description;
		room.IsPrivate = request.IsPrivate;
		room.CreatedAt = request.CreatedAt;
		room.CreatorUserId = request.CreatorUserId;

		return updatedRoom;
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
