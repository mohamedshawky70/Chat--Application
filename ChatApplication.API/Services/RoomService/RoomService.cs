using ChatApplication.API.DTOs.Room;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.RoomService;

public class RoomService(ApplicationDbContext context) : IRoomService
{
	private readonly ApplicationDbContext _context = context;
	public async Task<Result<IEnumerable<RoomResponse>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
	{
		var rooms = await _context.ChatRooms
			.Include(cr => cr.ChatRoomUsers)
			.ToListAsync(cancellationToken);
		if (rooms is null || rooms.Count == 0)
			return Result.Failure<IEnumerable<RoomResponse>>(RoomErrors.NoRoomsFound);

		var response = rooms.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result<RoomResponse>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
	{
		var room = await _context.ChatRooms
			.Where(cr=>cr.Id==roomId)
			.Include(cr=>cr.ChatRoomUsers)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);

		if (room == null)
			return Result.Failure<RoomResponse>(RoomErrors.NoRoomsFound);

		var response = room.MapToRoomResponse();
		return Result.Success(response);
	}
	public async Task<Result<IEnumerable<RoomResponse>>> GetUserRoomsAsync(string userId, CancellationToken cancellationToken = default)
	{
		var userRooms = await _context.ChatRoomUsers
			.Where(cru => cru.UserId == userId)
			.Include(cru => cru.ChatRoom)
			.ThenInclude(cr => cr.ChatRoomUsers)
			.Select(cru => cru.ChatRoom)
			.ToListAsync(cancellationToken);

		if (userRooms == null || userRooms.Count == 0)
			return Result.Failure<IEnumerable<RoomResponse>>(RoomErrors.UserHasNoRooms);

		var response = userRooms.MapToRoomResponse();
		return Result.Success(response);
	}

	public async Task<Result<RoomResponse>> CreateRoomAsync(CreatedRoomRequest request, CancellationToken cancellationToken = default)
	{
		var room = request.MapToChatRoom();
		var IsRoomExist = await _context.ChatRooms
			.AnyAsync(cr => cr.Name.ToLower() == request.Name.ToLower(), cancellationToken);
		if (IsRoomExist)
			return Result.Failure<RoomResponse>(RoomErrors.RoomAlreadyExists);

		await _context.ChatRooms.AddAsync(room, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		var roomUser = new ChatRoomUser
		{
			ChatRoomId = room.Id,
			UserId = request.CreatorUserId,
			IsAdmin = true,
			JoinedAt = DateTime.UtcNow
		};
		await _context.ChatRoomUsers.AddAsync(roomUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		var response = room.MapToRoomResponse();
		return Result.Success<RoomResponse>(response);
	}
}

