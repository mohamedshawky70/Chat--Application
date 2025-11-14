namespace ChatApplication.API.DTOs.Authentication;

public record _RefreshTokenRequest
(
	string Token,
	string RefreshToken
);
