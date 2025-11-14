namespace ChatApplication.API.DTOs.Authentication;

public class _ConfirmEmailRequestValidator : AbstractValidator<_ConfirmEmailRequest>
{
	public _ConfirmEmailRequestValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty();
		RuleFor(x => x.Code)
			.NotEmpty();
	}
}
