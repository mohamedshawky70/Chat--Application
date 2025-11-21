namespace ChatApplication.API.DTOs.Message;

public class EditMessageRequestValidator:AbstractValidator<EditMessageRequest>
{
	public EditMessageRequestValidator()
	{
		RuleFor(x => x.NewContent)
			.NotEmpty().WithMessage("Message content cannot be empty.")
			.MaximumLength(900).WithMessage("Message content cannot exceed 900 characters.");
	}
}
