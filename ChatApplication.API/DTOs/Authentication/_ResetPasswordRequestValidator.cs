using ChatApplication.API.Const;

namespace ChatApplication.API.DTOs.Authentication;

public class _ResetPasswordRequestValidator : AbstractValidator<_ResetPasswordRequest>
{
	public _ResetPasswordRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
		RuleFor(x => x.Code).NotEmpty();
		RuleFor(x => x.NewPassword)
			.Matches(RejexPattern.StrongPassword)
			.WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");


	}
}
