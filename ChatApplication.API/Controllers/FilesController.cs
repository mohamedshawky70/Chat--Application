using ChatApplication.API.DTOs.File;
using ChatApplication.API.Entites;
using ChatApplication.API.Services.FileService;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController(IFileService fileService) : ControllerBase
{
	private readonly IFileService _fileService = fileService;

	[HttpPost("")]
	public async Task<IActionResult> UploadProfileAvatar([FromForm] UploadProfileAvatarRequest request, CancellationToken cancellationToken)
	{
		var result = await _fileService.UploadProfileAvatarAsync(request, cancellationToken);
		return result.IsSuccess ? Ok() : NotFound(result.Error);
	}
}
