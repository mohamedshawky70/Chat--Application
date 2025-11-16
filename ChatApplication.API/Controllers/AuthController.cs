using ChatApplication.API.Authentication;
using ChatApplication.API.DTOs.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
	private readonly IAuthService _authService=authService;


	[HttpPost("")]
	public async Task<IActionResult> Login([FromBody] _LoginRequest request, CancellationToken cancellationToken)
	{
		var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : Unauthorized(result.Error);
	}

	[HttpPost("refresh-token")]
	public async Task<IActionResult> RefreshToken([FromBody] _RefreshTokenRequest request, CancellationToken cancellationToken)
	{
		var result = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
		return result.IsSuccess ? Ok(result.Value()) : Unauthorized(result.Error);
	}

	[HttpPost("revoke-refresh-token")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] _RefreshTokenRequest request, CancellationToken cancellationToken)
	{
		var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
		return result.IsSuccess ? Ok() : Unauthorized(result.Error);
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] _RegisterRequest request, CancellationToken cancellationToken)
	{
		var result = await _authService.RegisterAsync(request, cancellationToken);
		return result.IsSuccess ? Ok() : Conflict(result.Error);
	}

	[HttpPost("confirm-email")]
	public async Task<IActionResult> ConfirmEmail([FromBody] _ConfirmEmailRequest request)
	{
		var result = await _authService.ConfirmEmailAsync(request);
		return result.IsSuccess ? Ok() : BadRequest(result.Error);
	}

	[HttpPost("resend-confirmation-email")]
	public async Task<IActionResult> ResendConfirmationEmail([FromBody] _ResendConfirmationEmailRequest request)
	{
		var result = await _authService.ResendConfirmationEmailAsync(request);
		return result.IsSuccess ? Ok() : BadRequest(result.Error);
	}

	//Start forget password
	//send to by email code  
	[HttpPost("forgot-password")]
	public async Task<IActionResult> ForgotPassword([FromBody] _ForgetPasswordRequest request)
	{
		var result = await _authService.SendResetPasswordAsync(request);
		return result.IsSuccess ? Ok() : BadRequest(result.Error);
	}
	//This code redirect me to frontend to change pass 
	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] _ResetPasswordRequest request)
	{
		var result = await _authService.ResetPasswordAsync(request);
		return result.IsSuccess ? Ok() : Unauthorized(result.Error);
	}
	//End forget password

	//I will write change pass endpoint in account controller because i seperate logining endpoints (change pss) and unlogining                                                                                                                                                                                                                         m endpoints👆(login,register...). 
}
