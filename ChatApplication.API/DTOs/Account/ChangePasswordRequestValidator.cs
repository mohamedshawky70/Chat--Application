
namespace ChatApplication.API.DTOs.Account;

public class ChangePasswordRequestValidator:AbstractValidator<ChangePasswordRequest>
{
	public ChangePasswordRequestValidator()
	{
		RuleFor(x => x.CurrentPassword)
			.NotEmpty()
			.NotEqual(x => x.NewPassword)
			.WithMessage("New password can't be the same current password");
		RuleFor(x => x.NewPassword)
			.NotEmpty()
			.Matches(RejexPattern.StrongPassword)
			.WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");
	}
}
