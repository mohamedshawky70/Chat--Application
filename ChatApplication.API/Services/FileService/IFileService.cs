using ChatApplication.API.DTOs.File;
using System.Threading.Tasks;

namespace ChatApplication.API.Services.FileService;

public interface IFileService
{
	Task<Result> UploadProfileAvatarAsync(UploadProfileAvatarRequest request , CancellationToken cancellationToken=default);

	Task<Result<Guid>> UploadFileAsync(UploadFileRquest request,int messageId, CancellationToken cancellationToken = default);

	Task<(byte[] fileContent, string ContentType, string fileName)>DownloadFileAsync(Guid id, CancellationToken cancellationToken = default);
}
