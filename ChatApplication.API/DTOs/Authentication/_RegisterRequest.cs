namespace ChatApplication.API.DTOs.Authentication;

public record _RegisterRequest
(
	string UserName,
	string Email,
	string Password
);
