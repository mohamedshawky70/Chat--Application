using ChatApplication.API.DTOs.Message;
using ChatApplication.API.DTOs.Room;
using System.Threading.Tasks;

namespace ChatApplication.API.Services.RoomService;

public interface IRoomService
{
	Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<RoomResponse>>> GetUserRoomsAsync(string userId, CancellationToken cancellationToken = default);

	Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default);

	Task<Result<RoomResponse>> CreateRoomAsync(FormRoomRequest request, CancellationToken cancellationToken = default);
	
	Task<Result<RoomResponse>> UpdateRoomAsync(int roomId, FormRoomRequest request, CancellationToken cancellationToken = default);
	
	Task<Result> JoinRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default);
	
	Task<Result> LeaveRoomAsync(string userId,int roomId, CancellationToken cancellationToken = default);

	Task<Result> AddUserToRoomAsync(string userId, int roomId,CancellationToken cancellationToken = default);

    Task<Result> RemoveUserFromRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default);

	Task<Result<PaginationList<MessageResponse>>> GetRoomMessagesAsync(int roomId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);

	Task<Result> UserTypingAsync(string userId, int roomId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<MessageResponse>>> GetUnreadMessagesAsync(int roomId, CancellationToken cancellationToken = default);

	Task<Result<int>> GetUnreadMessagesCountAsync(int roomId, CancellationToken cancellationToken = default);

	Task<Result> MarkAllMessageAsReadAsync(int roomId, CancellationToken cancellationToken = default);

	Task<Result> MarkMessageAsReadAsync(int messageId, int roomId, CancellationToken cancellationToken = default);

	Task<Result> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default);
}
