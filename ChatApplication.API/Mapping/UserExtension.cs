using ChatApplication.API.DTOs.User;

namespace ChatApplication.API.Mapping;


public static class UserExtension
{
	public static UserProfileResponse MapToResponse(this User user)
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
}

