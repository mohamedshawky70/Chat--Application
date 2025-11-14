namespace ChatApplication.API.DTOs.Authentication;

public record _ConfirmEmailRequest
(
	string UserId,
	string Code
);
