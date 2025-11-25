using ChatApplication.API.Abstractions;
using ChatApplication.API.DTOs.Authentication;
using ChatApplication.API.Errors;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplication.API.Authentication;

public class AuthService : IAuthService
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;
	private readonly IJwtProvider _jwtProvider;
	private readonly IEmailSender _emailSender;
	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly int _refreshTokenExpiryDays = 14;
	public AuthService(UserManager<User> userManager, IJwtProvider jwtProvider, SignInManager<User> signInManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_jwtProvider = jwtProvider;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_httpContextAccessor = httpContextAccessor;
	}
	//Login
	public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
	{
		//check email
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null)
			return Result.Failure<AuthResponse>(UserError.InvalidCredential);

		//if(user.IsDeleted) Comment to can activate his account
		//	return Result.Failure<AuthResponse>(UserError.UserDeleted);
		//check password
		var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
		if (result.Succeeded)
		{
			//send roles of user to token for front end 
			var userRoles = await _userManager.GetRolesAsync(user);
			//generate JWT taken
			var (taken, expiresIn) = _jwtProvider.GenerateTaken(user, userRoles);

			//generate refresh token
			var refreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
			user.RefreshTokens.Add(new RefreshToken
			{
				Token = refreshToken,
				ExpiresIn = refreshTokenExpiration
			});
			await _userManager.UpdateAsync(user);

			var response=new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, taken, expiresIn, refreshToken, refreshTokenExpiration);

			return Result.Success(response);
		}
		// يإما اللباس غلط او مش كونفرمد
		//او لوك
		var error =
			 result.IsLockedOut
			? UserError.UserLockOut
			: result.IsNotAllowed
			? UserError.EmailNotConfirmed
			: UserError.InvalidCredential;

		return Result.Failure<AuthResponse>(error);
	}
	public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateTaken(token);
		//لو الاكسباير ديت انتهي هيرجع نل
		if (userId is null)
			return Result.Failure<AuthResponse>(UserError.InvalidRefreshToken);

		var user = await _userManager.FindByIdAsync(userId);
		//المنطق انه مش هيرجع نل لكن لا نثق ابدا في اليوزر اللعين
		if (user is null)
			return Result.Failure<AuthResponse>(UserError.InvalidRefreshToken);

		//if(user.IsDeleted)
		//	return Result.Failure<AuthResponse>(UserError.UserDeleted);

		if (user.LockoutEnd > DateTime.UtcNow)
			return Result.Failure<AuthResponse>(UserError.UserLockOut);


		var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);
		if (userRefreshToken is null)
			return Result.Failure<AuthResponse>(UserError.InvalidRefreshToken);
		userRefreshToken.RevokedIn = DateTime.UtcNow;//إلغي الرفرش القديمه

		//Regenerate new token
		var userRoles = await _userManager.GetRolesAsync(user);
		var (newToken, expiresIn) = _jwtProvider.GenerateTaken(user, userRoles);

		//generate new refresh taken
		var newRefreshToken = GenerateRefreshToken();
		var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
		user.RefreshTokens.Add(new RefreshToken
		{
			Token = newRefreshToken,
			ExpiresIn = refreshTokenExpiration
		});
		await _userManager.UpdateAsync(user);

		var response=new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);
		return Result.Success(response);

	}
	public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateTaken(token);
		//لو الاكسباير ديت انتهي هيرجع نل
		if (userId is null)
			return Result.Failure(UserError.InvalidRefreshToken_Token);

		var user = await _userManager.FindByIdAsync(userId);
		//المنطق انه مش هيرجع نل لكن لا نثق ابدا في اليوزر اللعين
		if (user is null)
			return Result.Failure(UserError.InvalidRefreshToken_Token);

		var userRefreshToken = user.RefreshTokens.SingleOrDefault(u => u.Token == refreshToken);
		if (userRefreshToken is null)
			return Result.Failure(UserError.InvalidRefreshToken_Token);

		userRefreshToken.RevokedIn = DateTime.UtcNow;//إلغي الرفرش القديمه
		await _userManager.UpdateAsync(user);
		return Result.Success();
	}
	public async Task<Result> RegisterAsync(_RegisterRequest request, CancellationToken cancellationToken = default)
	{
		var email = await _userManager.FindByEmailAsync(request.Email);
		if (email is not null)
			return Result.Failure(UserError.DuplicateUser);
		var user = new User()
		{
			Email = request.Email,
			UserName = request.Email,
			FirstName = request.FirstName,
			LastName = request.LastName
		};
		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			//Generate code to send it in email to user to confirmed
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			//Start Send Email
			await SendEmailConfirmation(user, code);
			//End Send Email
			  return Result.Success();
			//return code;
		}
		var error = result.Errors.First();
		return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status409Conflict));
	}
	public async Task<Result> ConfirmEmailAsync(_ConfirmEmailRequest request)
	{
		var user = await _userManager.FindByIdAsync(request.UserId);
		if (user is null)
			return Result.Failure(UserError.InvalidCode);
		if (user.EmailConfirmed)
			return Result.Failure(UserError.DuplicateConfirmed);

		var code = request.Code;
		try
		{
			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)); //فك تشفيرته
		}
		catch (Exception)
		{
			return Result.Failure(UserError.InvalidCode);

		}
		var result = await _userManager.ConfirmEmailAsync(user, code); //verify
		if (result.Succeeded)
			return Result.Success();

		var error = result.Errors.First();
		return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
	}
	public async Task<Result> ResendConfirmationEmailAsync(_ResendConfirmationEmailRequest request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user is null)
			 return Result.Success();
		if (user.EmailConfirmed)
			return Result.Failure(UserError.DuplicateConfirmed);

		var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
		await SendEmailConfirmation(user, code);
		return Result.Success();

	}
	public async Task<Result> SendResetPasswordAsync(_ForgetPasswordRequest request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user is null)
			 return Result.Success();
		if (!user.EmailConfirmed)
			return Result.Failure(UserError.EmailNotConfirmed);
		var code = await _userManager.GeneratePasswordResetTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
		await SendResetPassword(user, code);
		return Result.Success();
	}
	public async Task<Result> ResetPasswordAsync(_ResetPasswordRequest request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user is null || !user.EmailConfirmed)
			return Result.Failure(UserError.InvalidCode);
		IdentityResult result;
		try
		{
			var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code)); //فك تشفيرته
			result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
		}
		catch (FormatException)
		{
			result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
		}
		if (result.Succeeded)
			return Result.Success();

		var error = result.Errors.First();
		return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
	}

	private static string GenerateRefreshToken() =>
		Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
	//Start Send Email
	private async Task SendEmailConfirmation(User user, string code)
	{
		var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;//https://localhost:7123/ اليو ار ال اللي مبعوتلي مع الريكويست في الهيدر
		var TempPath = $"{Directory.GetCurrentDirectory()}/Templates/EmailConfirmation.html";//مكان التمبلت اللي هتتبعت
		StreamReader streamReader = new StreamReader(TempPath);//للتعامل مع هذه التمبلت
		var body = streamReader.ReadToEnd();//إقراها للاخر
		streamReader.Close();
		body = body
			.Replace("[name]", $"{user.FirstName} {user.LastName}")
			.Replace("[action_url]", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}");//الفرونت هيعرفني شكل اليو ار ال ده مثال مش اكتر

		BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅Confirm your email", body));
		await Task.CompletedTask;
	}
	private async Task SendResetPassword(User user, string code)
	{
		var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;// اليو ار ال اللي مبعوتلي مع الريكويست في الهيدر
		var TempPath = $"{Directory.GetCurrentDirectory()}/Templates/ForgetPassword.html";//مكان التمبلت اللي هتتبعت
		StreamReader streamReader = new StreamReader(TempPath);//للتعامل مع هذه التمبلت
		var body = streamReader.ReadToEnd();//إقراها للاخر
		streamReader.Close();
		body = body
			.Replace("{{name}}", $"{user}")
			.Replace("{{action_url}}", $"{origin}/auth/ForgetPassword?email={user.Email}&code={code}");//الفرونت هيعرفني شكل اليو ار ال ده مثال مش اكتر

		BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Reset your password", body));
		await Task.CompletedTask;
	}
	//End Send Email
}