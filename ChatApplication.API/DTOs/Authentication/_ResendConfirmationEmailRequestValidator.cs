
namespace ChatApplication.API.DTOs.Authentication;

public class _ResendConfirmationEmailRequestValidator : AbstractValidator<_ResendConfirmationEmailRequest>
{
	public _ResendConfirmationEmailRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
	}
}
