using ChatApplication.API.Abstractions;
using ChatApplication.API.DTOs.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Authentication;

public interface IAuthService
{
	Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);

	Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

	Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

	Task<Result> RegisterAsync(_RegisterRequest request, CancellationToken cancellationToken = default);

	Task<Result> ConfirmEmailAsync(_ConfirmEmailRequest request);

	Task<Result> ResendConfirmationEmailAsync(_ResendConfirmationEmailRequest request);

	Task<Result> SendResetPasswordAsync(_ForgetPasswordRequest request);

	Task<Result> ResetPasswordAsync(_ResetPasswordRequest request);
}
