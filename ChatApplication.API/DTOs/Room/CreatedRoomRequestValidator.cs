namespace ChatApplication.API.DTOs.Room;

public class CreatedRoomRequestValidator : AbstractValidator<CreatedRoomRequest>
{
	public CreatedRoomRequestValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Room name is required.")
			.MaximumLength(200).WithMessage("Room name must not exceed 100 characters.");
		RuleFor(x => x.Description)
			.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
		RuleFor(x => x.CreatedAt)
			.LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future.");
	}
}
