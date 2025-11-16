namespace ChatApplication.API.DTOs.File;

public record UploadProfileAvatarRequest
(
	  IFormFile? Avatar ,
	  string UserId =null!
);
 