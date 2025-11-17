namespace ChatApplication.API.DTOs.User;

public record UserProfileRequest
(
	string FirstName,
	string LastName,
	string? PhoneNumber
);
