using ChatApplication.API.Settings;

namespace ChatApplication.API.DTOs.Message;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
	public SendMessageRequestValidator()
	{

		RuleFor(x => x.File)
			.Must(x => x?.Length <= FileSettings.FileMaxSizeInMb)
			.WithMessage($"Max image size {FileSettings.FileMaxSizeInMb / 1024 / 1024} MB")
			.When(x => x.File is not null);//علشان متبلعش نل في اللينث
										   // لو غير الاكتنشن بتاع ملف الجافاسكربت هيقبله عادي علشان كده الافضل تعتمد علي السيجنتشر
		RuleFor(x => x.File)
			.Must(x =>
			{
				var ExtensionImage = Path.GetExtension(x?.FileName.ToLower());//ممكن يجولك كابتل
				return FileSettings.AllowedImagesExtension.Contains(ExtensionImage); // true or false
			})
			.WithMessage("Not allowed image extension")
			.When(x => x.File is not null);
	}
}
