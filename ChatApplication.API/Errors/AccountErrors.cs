namespace ChatApplication.API.Errors;

public static class AccountErrors
{
	public static readonly Error AccountActivated = new Error("User.AccountActivated", "Account already activated", StatusCodes.Status400BadRequest);

	public static readonly Error AccountDeactivated = new Error("User.AccountDeactivated", "Account already deactivated", StatusCodes.Status400BadRequest);
}
