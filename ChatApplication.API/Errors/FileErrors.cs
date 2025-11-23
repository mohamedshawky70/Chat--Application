using Microsoft.AspNetCore.Http;

namespace ChatApplication.API.Errors;

public static class FileErrors
{
	public readonly static Error FileNotFound = new("Files.FileNotFound", "The requested file was not found on the server.",StatusCodes.Status404NotFound);
}
