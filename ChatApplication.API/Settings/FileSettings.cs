namespace ChatApplication.API.Settings;

public static class FileSettings
{
	public const int FileMaxSizeInMb = 1 * 1024 * 1024; //KB===>B===>MB (1MB)
	public static readonly string[] AllowedImagesExtension = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

	public const long _maxFileSize = 50 * 1024 * 1024; // 50 MB 
	public static readonly string[] _allowedExtensions =
	{
		".jpg", ".jpeg", ".png", ".gif", ".webp",    // images
        ".mp4", ".mov", ".avi", ".webm",             // videos
        ".mp3", ".wav", ".ogg", ".m4a",              // audio
        ".pdf", ".doc", ".docx", ".txt", ".zip"      // documents
    };
}
