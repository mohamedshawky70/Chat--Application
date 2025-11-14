namespace ChatApplication.API.DTOs.Authentication;

public record _ResetPasswordRequest
(
	string Email,
	string Code,
	string NewPassword
);
