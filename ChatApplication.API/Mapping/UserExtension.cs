using ChatApplication.API.DTOs.Account;
using ChatApplication.API.DTOs.User;

namespace ChatApplication.API.Mapping;


public static class UserExtension
{
	public static UserProfileResponse MapToUserProfileResponse(this User user)
	{
		return new UserProfileResponse(
			user.FirstName,
			user.LastName,
			user.PhoneNumber,
			user.Avatar
		);
	}

	public static User MapToUser(this UserProfileRequest request, User user)
	{
		var updatedUser = user;

		user.FirstName = request.FirstName;
		user.LastName = request.LastName;
		user.PhoneNumber = request.PhoneNumber;
		return updatedUser;
	}

	public static UserResponse MapToUserResponse(this User user)
	{
		return new UserResponse(
			 user.FirstName,
			 user.LastName,
			 user.PhoneNumber,
			 user.IsOnline,
			 user.Avatar
		);
	}
	
	public static IEnumerable<UserResponse> MapToUserResponse(this IEnumerable<User> users)
	{
		return users.Select(u => u.MapToUserResponse());
		//return users.Select(u => new UserResponse
		//(
		//	 u.FirstName,
		//	 u.LastName,
		//	 u.PhoneNumber,
		//	 u.IsOnline,
		//	 u.Avatar
		//));
	}

}

