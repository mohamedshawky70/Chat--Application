using ChatApplication.API.DTOs.Room;

namespace ChatApplication.API.Services.RoomService;

public interface IRoomService
{
	Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<RoomResponse>>> GetUserRoomsAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<RoomResponse>> CreateRoomAsync(CreatedRoomRequest request, CancellationToken cancellationToken = default);

	Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default);
}
