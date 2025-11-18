namespace ChatApplication.API.DTOs.User;

public record UserResponse
(
	 string FirstName,
     string LastName,
     string PhoneNumber,
	 bool IsOnline,
     string? Avatar
);
