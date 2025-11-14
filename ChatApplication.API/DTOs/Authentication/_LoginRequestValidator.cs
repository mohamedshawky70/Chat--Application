namespace ChatApplication.API.DTOs.Authentication;

public class _LoginRequestValidator : AbstractValidator<_LoginRequest>
{
	public _LoginRequestValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress();
		RuleFor(x => x.Password)
			.NotEmpty();
	}
}
