using ChatApplication.API.DTOs.File;

namespace ChatApplication.API.Services.FileService;

public interface IFileService
{
	Task<Result> UploadProfileAvatarAsync(UploadProfileAvatarRequest request , CancellationToken cancellationToken=default);

	Task<Result<Guid>> UploadFileAsync(UploadFileRquest request,int messageId, CancellationToken cancellationToken = default);
}
