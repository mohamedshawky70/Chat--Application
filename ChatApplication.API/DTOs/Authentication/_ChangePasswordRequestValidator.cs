using ChatApplication.API.Const;

namespace ChatApplication.API.DTOs.Authentication;

public class _ChangePasswordRequestValidator : AbstractValidator<_ChangePasswordRequest>
{
	public _ChangePasswordRequestValidator()
	{
		RuleFor(x => x.CurrentPassword).NotEmpty();

		RuleFor(x => x.NewPassword).NotEmpty()
			.NotEqual(x => x.CurrentPassword)
			.WithMessage("New password can't be the same current password")
			.Matches(RejexPattern.StrongPassword)
			.WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");

	}
}
