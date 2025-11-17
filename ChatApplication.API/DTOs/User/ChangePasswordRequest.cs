namespace ChatApplication.API.DTOs.User;

public record ChangePasswordRequest
(
	string CurrentPassword,
	string NewPassword
);