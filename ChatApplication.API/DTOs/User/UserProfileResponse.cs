namespace ChatApplication.API.DTOs.User;

public record UserProfileResponse
(
	string FirstName,
	string LastName,
	string PhonNumber,
	string? Avatar
);
