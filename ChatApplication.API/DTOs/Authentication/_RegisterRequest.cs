namespace ChatApplication.API.DTOs.Authentication;

public record _RegisterRequest
(
	string FirstName,
	string LastName,
	string Email,
	string Password
);
