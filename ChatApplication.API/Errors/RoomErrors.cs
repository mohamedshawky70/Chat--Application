namespace ChatApplication.API.Errors;

public static class RoomErrors
{
	public readonly static Error NoRoomsFound = new("Rooms.NoRoomsFound","No chat rooms were found.",StatusCodes.Status404NotFound);
	public readonly static Error UserHasNoRooms = new("Rooms.UserHasNoRooms","User has No rooms.",StatusCodes.Status404NotFound);
	public readonly static Error RoomAlreadyExists = new("Rooms.RoomAlreadyExists","Room already exists.",StatusCodes.Status409Conflict);
}
