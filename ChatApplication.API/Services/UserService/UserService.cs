using ChatApplication.API.DTOs.User;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.UserService;

public class UserService(ApplicationDbContext context, UserManager<User> userManager) : IUserServeic
{
	private readonly ApplicationDbContext _context = context;
	private readonly UserManager<User> _userManager=userManager;

	public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync(FilterRequest request, CancellationToken cancellationToken = default)
	{
		var users = _context.Users.AsNoTracking();

		//Searching
		var SearchValueTrim=request.SearchValue.Trim(); //Remove space in front and back ("MO" != " MO")
		if (!string.IsNullOrEmpty(SearchValueTrim))
			users = users.Where(u => (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(SearchValueTrim)) ||
									 (!string.IsNullOrEmpty(u.LastName)  && u.LastName.Contains(SearchValueTrim)));
		//Ordering
		users = users
			.OrderByDescending(u=>u.IsOnline)
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

		var response=user.MapToUserResponse();
		return Result.Success(response);
	}
	public async Task<Result<IEnumerable<UserResponse>>> GetOnlineUsersAsync(CancellationToken cancellationToken = default)
	{
		var users = await _context.Users
			.Where(u=>u.IsOnline)
			.ToListAsync(cancellationToken);
		if(users.Count==0)
			return Result.Failure<IEnumerable<UserResponse>>(UserError.NoActiveUser);

		var response= users.MapToUserResponse();
		return Result.Success(response);
	}
}
