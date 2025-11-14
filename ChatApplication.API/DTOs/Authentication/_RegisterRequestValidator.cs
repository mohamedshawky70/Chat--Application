using ChatApplication.API.Const;

namespace ChatApplication.API.DTOs.Authentication;
public class _RegisterRequestValidator : AbstractValidator<_RegisterRequest>
{
	public _RegisterRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress();
		RuleFor(x => x.FirstName).NotEmpty().Length(3, 100);//default in Asp.NetUser is 100
		RuleFor(x => x.LastName).NotEmpty().Length(3, 100);
		RuleFor(x => x.Password)
			.Matches(RejexPattern.StrongPassword)
			.WithMessage("Password must contains atleast 8 digits, one Uppercase,one Lowercase and NunAlphanumeric");
	}
}
