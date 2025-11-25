namespace ChatApplication.API.DTOs.Account;

public record UserProfileResponse
(
	string FirstName,
	string LastName,
	string? PhonNumber,
	string? Avatar,
	string? Bio
);
