using ChatApplication.API.Settings;
using FluentValidation;

namespace ChatApplication.API.DTOs.File;

public class UploadFileRquestValidator :AbstractValidator<UploadFileRquest>
{
	public UploadFileRquestValidator()
	{
		RuleFor(x=>x.File)
			.Must(x=> x.Length <= FileSettings._maxFileSize)
			.WithMessage($"Max file size {FileSettings._maxFileSize / 1024 / 1024} MB")
			.When(x=> x.File is not null);

		RuleFor(x=>x.File)
			.Must(x=> FileSettings._allowedExtensions.Contains(Path.GetExtension(x.FileName.ToLower())))
			.WithMessage("Not allowed file extension")
			.When(x=> x.File is not null);
	}
}
