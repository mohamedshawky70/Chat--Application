using ChatApplication.API.Services.AccountService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]

[Authorize]
public class AccountsController(IAccountService accountService) : ControllerBase
{
	private readonly IAccountService _accountService = accountService;

	[HttpGet("profile")]
	public async Task<IActionResult> UserProfile(CancellationToken cancellationToken)
	{
		var userId=User.FindFirstValue(ClaimTypes.NameIdentifier)!;
		var result = await _accountService.UserProfileAsync(userId, cancellationToken);
		return result.IsSuccess? Ok(result.Value()) : NotFound(result);
	}
}
