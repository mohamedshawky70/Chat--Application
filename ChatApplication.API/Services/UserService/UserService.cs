using ChatApplication.API.Hubs;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.UserService;

public class UserService(ApplicationDbContext context, UserManager<User> userManager, IHubContext<ChatHub> hubContext) : IUserServeic
{
	private readonly ApplicationDbContext _context = context;
	private readonly UserManager<User> _userManager = userManager;
	private readonly IHubContext<ChatHub> _hubContext = hubContext;

	public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync(FilterRequest request, CancellationToken cancellationToken = default)
	{
		var users = _context.Users.AsNoTracking();

		//Searching
		var SearchValueTrim = request.SearchValue.Trim(); //Remove space in front and back ("MO" != " MO")
		if (!string.IsNullOrEmpty(SearchValueTrim))
			users = users.Where(u => (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(SearchValueTrim)) ||
									 (!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(SearchValueTrim)));
		//Ordering
		users = users
			.OrderByDescending(u => u.IsOnline)
			.ThenBy(u => u.FirstName)
			.ThenBy(u => u.LastName);

		var response = users.MapToUserResponse();
		return Result.Success(response);
	}

	public async Task<Result<UserResponse>> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<UserResponse>(UserError.UserNotFound);

		var response = user.MapToUserResponse();
		return Result.Success(response);
	}
	public async Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersAsync(CancellationToken cancellationToken = default)
	{
		var users = await _context.Users
			.Where(u => u.IsOnline)
			.ToListAsync(cancellationToken);
		if (users.Count == 0)
			return Result.Failure<IEnumerable<UserResponse>>(UserError.NoActiveUser);

		var response = users.MapToUserResponse();
		return Result.Success(response);
	}

	public async Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersInRoomAsync(int roomId, CancellationToken cancellationToken = default)
	{

		var users = await (from cru in _context.ChatRoomUsers
						   join u in _context.Users
						   on cru.UserId equals u.Id
						   where cru.ChatRoomId == roomId && u.IsOnline
						   select u).ToListAsync(cancellationToken);
		if (users == null || users.Count == 0)
			return Result.Failure<IEnumerable<UserResponse>>(UserError.NoActiveUser);

		var response = users.MapToUserResponse();
		return Result.Success(response);
	}

	public async Task<Result<UserResponse>> SetAsAdminAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<UserResponse>(UserError.UserNotFound);

		var room = await _context.ChatRooms.FindAsync(roomId, cancellationToken);
		if (room == null)
			return Result.Failure<UserResponse>(RoomErrors.NoRoomsFound);

		var chatRoomUser = await _context.ChatRoomUsers
			.FirstOrDefaultAsync(cru => cru.UserId == userId && cru.ChatRoomId == roomId, cancellationToken);
		if (chatRoomUser == null)
			return Result.Failure<UserResponse>(UserError.UserNotFoundInRoom);

		if (chatRoomUser.IsAdmin)
			return Result.Failure<UserResponse>(UserError.UserIsAlreadyAdmin);

		chatRoomUser.IsAdmin = true;
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Clients.Group($"room-{roomId}")
			.SendAsync("UserRoleChanged", new { UserId = userId, Role = "Admin" }, cancellationToken);

		var response = user.MapToUserResponse();
		return Result.Success(response);
	}

	public async Task<Result<UserResponse>> RemoveFromAdminAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<UserResponse>(UserError.UserNotFound);

		var room = await _context.ChatRooms.FindAsync(roomId, cancellationToken);
		if (room == null)
			return Result.Failure<UserResponse>(RoomErrors.NoRoomsFound);

		var chatRoomUser = await _context.ChatRoomUsers
			.FirstOrDefaultAsync(cru => cru.UserId == userId && cru.ChatRoomId == roomId, cancellationToken);
		if (chatRoomUser == null)
			return Result.Failure<UserResponse>(UserError.UserNotFoundInRoom);

		chatRoomUser.IsAdmin = false;
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Clients.Group($"room-{roomId}")
		   .SendAsync("UserRoleChanged", new { UserId = userId, Role = "member" }, cancellationToken);

		var response = user.MapToUserResponse();
		return Result.Success(response);
	}

	public async Task<Result<bool>> IsAdminInRoomAsync(string userId, int roomId, CancellationToken cancellationToken = default)
	{
		var isAdmin=await _context.ChatRoomUsers
			.AnyAsync(cru => cru.UserId == userId && cru.ChatRoomId == roomId && cru.IsAdmin, cancellationToken);

		return Result.Success(isAdmin);
	}

	public async Task<Result<DateTime>> GetLastSeenAsync(string userId, CancellationToken cancellationToken = default)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return Result.Failure<DateTime>(UserError.UserNotFound);

		return Result.Success(user.LastSeen);
	}

	public async Task<Result> BlockUserAsync(string blockerId, string blockedId, CancellationToken cancellationToken = default)
	{
		if(blockerId==blockedId)
			return Result.Failure(UserError.SelfBlocked);

		var blocker =await _context.Users.FindAsync(blockerId, cancellationToken);
		var blocked =await _context.Users.FindAsync(blockedId, cancellationToken);
		if (blocker == null || blocked == null)
			return Result.Failure(UserError.UserNotFound);

		var existingBlock = await _context.BlockedUsers
			.FirstOrDefaultAsync(bu => bu.BlockerId == blockerId && bu.BlockedId == blockedId, cancellationToken);
		if (existingBlock != null)
			return Result.Failure(UserError.UserBlocked);

		var blockedUser = new BlockedUser()
		{
			BlockerId = blockerId,
			BlockedId = blockedId,
			BlockedAt = DateTime.UtcNow
		};
		await _context.BlockedUsers.AddAsync(blockedUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
	
	public async Task<Result> UnBlockUserAsync(string blockerId, string blockedId, CancellationToken cancellationToken = default)
	{
		if(blockerId==blockedId)
			return Result.Failure(UserError.SelfBlocked);

		var blocker =await _context.Users.FindAsync(blockerId, cancellationToken);
		var blocked =await _context.Users.FindAsync(blockedId, cancellationToken);
		if (blocker == null || blocked == null)
			return Result.Failure(UserError.UserNotFound);

		var existingBlock = await _context.BlockedUsers
			.FirstOrDefaultAsync(bu => bu.BlockerId == blockerId && bu.BlockedId == blockedId, cancellationToken);
		if (existingBlock == null)
			return Result.Failure(UserError.UserBlocked);

		_context.BlockedUsers.Remove(existingBlock);
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}

	public async Task<Result<bool>> IsBlockedAsync(string userId1, string userId2, CancellationToken cancellationToken = default)
    {
		if(userId1==userId2)
			return Result.Failure<bool>(UserError.SelfBlocked);

        var blocked = await _context.BlockedUsers
            .AnyAsync(b =>
                (b.BlockerId == userId1 && b.BlockedId == userId2) ||
                (b.BlockerId == userId2 && b.BlockedId == userId1), cancellationToken);

        return Result.Success(blocked);
    }

}
