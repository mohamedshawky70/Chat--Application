namespace ChatApplication.API.DTOs.Authentication;

public record _ChangePasswordRequest
(
	string CurrentPassword,
	string NewPassword
);
