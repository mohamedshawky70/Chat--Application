namespace ChatApplication.API.DTOs.Authentication;

public class _ForgetPasswordRequestValidator : AbstractValidator<_ForgetPasswordRequest>
{
	public _ForgetPasswordRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
	}
}
