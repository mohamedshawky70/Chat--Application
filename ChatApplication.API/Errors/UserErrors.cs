namespace ChatApplication.API.Errors;

public static class UserError
{
	public static readonly Error InvalidCredential = new Error("User.InvalidCredential", "Invalid email/password", StatusCodes.Status401Unauthorized);
	public static readonly Error InvalidRefreshToken = new Error("User.InvalidRefreshToken", "InvalidRefreshToken~", StatusCodes.Status401Unauthorized);
	public static readonly Error InvalidRefreshToken_Token = new Error("User.InvalidRefreshToken/Token", "InvalidRefreshToken/Token", StatusCodes.Status401Unauthorized);
	public static readonly Error DuplicateUser = new Error("User.DuplicateUser", "User with the same email already existed", StatusCodes.Status409Conflict);
	public static readonly Error EmailNotConfirmed = new Error("User.EmailNotConfirmed", "Email not confirmed", StatusCodes.Status401Unauthorized);
	public static readonly Error InvalidCode = new Error("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
	public static readonly Error DuplicateConfirmed = new Error("User.DuplicateConfirmed", "This email already confirmed", StatusCodes.Status400BadRequest);
	public static readonly Error UserIsDisable = new Error("User.UserIsDisable", "User is disable", StatusCodes.Status401Unauthorized);
	public static readonly Error UserLockOut = new Error("User.UserLockOut", "User is lockOut", StatusCodes.Status401Unauthorized);
	public static readonly Error UserNotFound = new Error("User.NotFound", " User not found", StatusCodes.Status404NotFound);
	public static readonly Error InvalidRole = new Error("User.InvalidRole", "Invalid role", StatusCodes.Status400BadRequest);
	public static readonly Error UpdateFailed = new Error("User.UpdateFailed", "Update failed", StatusCodes.Status400BadRequest);
	public static readonly Error ChangePasswordFailed = new Error("User.ChangePasswordFailed", "Change password failed", StatusCodes.Status400BadRequest);
	public static readonly Error NoActiveUser = new Error("User.NoActiveUser", "No Active User", StatusCodes.Status404NotFound);
	public static readonly Error UserNotFoundInRoom = new Error("User.UserNotFoundInRoom", "User not found in room", StatusCodes.Status404NotFound);
	public static readonly Error UserIsAlreadyAdmin = new Error("User.UserIsAlreadyAdmin", "User is already admin", StatusCodes.Status409Conflict);
	public static readonly Error UserBlocked = new Error("User.UserBlocked", "User is already blocked", StatusCodes.Status409Conflict);
	public static readonly Error UserUnBlocke = new Error("User.UserUnBlocke", "User is already unblocked", StatusCodes.Status409Conflict);
	public static readonly Error SelfBlocked = new Error("User.SelfBlocked", "Can't block you self", StatusCodes.Status409Conflict);
	public static readonly Error UserDeleted = new Error("User.UserDeleted", "User is deleted", StatusCodes.Status409Conflict);
	public static readonly Error Ex = new Error("Internal server error", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1", StatusCodes.Status500InternalServerError);
}