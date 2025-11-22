namespace ChatApplication.API.DTOs.File;

public record UploadFileRquest
(
	IFormFile File,
	string? Caption = null
);
