using ChatApplication.API.DTOs.Account;

namespace ChatApplication.API.Mapping;


public static class UserExtension
{
	public static UserProfileResponse MapToUserProfileResponse(this User user)
	{
		return new UserProfileResponse(
			user.FirstName,
			user.LastName,
			user.PhoneNumber,
			user.Avatar,
			user.Bio
		);
	}

	public static User MapToUser(this UserProfileRequest request, User user)
	{
		var updatedUser = user;

		user.FirstName = request.FirstName;
		user.LastName = request.LastName;
		user.PhoneNumber = request.PhoneNumber;
		user.Bio = request.Bio;
		return updatedUser;
	}

	public static UserResponse MapToUserResponse(this User user)
	{
		return new UserResponse(
			 user.FirstName,
			 user.LastName,
			 user.PhoneNumber,
			 user.IsOnline,
			 user.IsDeleted,
			 user.Avatar,
			 user.Bio
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

