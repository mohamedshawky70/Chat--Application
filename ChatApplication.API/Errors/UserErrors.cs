using ChatApplication.API.Abstractions;
using System;

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
	public static readonly Error UserIsDisable = new Error("User.UserIsDisable", "User Is Disable", StatusCodes.Status401Unauthorized);
	public static readonly Error UserLockOut = new Error("User.UserLockOut", "User Is LockOut", StatusCodes.Status401Unauthorized);
	public static readonly Error UserNotFound = new Error("User.NotFound", " User Not Found", StatusCodes.Status401Unauthorized);
	public static readonly Error InvalidRole = new Error("User.InvalidRole", "Invalid Role", StatusCodes.Status400BadRequest);
	public static readonly Error UpdateFailed = new Error("User.UpdateFailed", "Update Failed", StatusCodes.Status400BadRequest);
	public static readonly Error ChangePasswordFailed = new Error("User.ChangePasswordFailed", "Change Password Failed", StatusCodes.Status400BadRequest);
	public static readonly Error NoActiveUser = new Error("User.NoActiveUser", "No Active User", StatusCodes.Status404NotFound);
}