namespace ChatApplication.API.DTOs.Authentication;

public class _RefreshTokenRequestValidator : AbstractValidator<_RefreshTokenRequest>
{
	public _RefreshTokenRequestValidator()
	{
		RuleFor(x => x.Token).NotEmpty();
		RuleFor(x => x.RefreshToken).NotEmpty();
	}
}
