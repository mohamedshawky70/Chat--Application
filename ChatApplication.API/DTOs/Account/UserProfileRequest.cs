namespace ChatApplication.API.DTOs.Account;

public record UserProfileRequest
(
	string FirstName,
	string LastName,
	string? PhoneNumber
);
