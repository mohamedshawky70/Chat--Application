using ChatApplication.API.DTOs.File;
using ChatApplication.API.Entites;

namespace ChatApplication.API.Mapping;

public static class UploadFileExtension
{
	public static UploadFile MapToUploadFile(this UploadFileRquest request)
	{
		return new UploadFile()
		{
			FileName = request.File.FileName,
			ContentType = request.File.ContentType, //image/png.. or video/mp4 or any file with any extinsion
			FileExtension = Path.GetExtension(request.File.FileName),
		};
	}
}
