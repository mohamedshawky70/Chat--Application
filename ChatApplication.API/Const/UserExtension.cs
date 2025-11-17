using System.Security.Claims;

namespace ChatApplication.API.Const;

public static class UserExtension
{
	public static string GetUserId(this ClaimsPrincipal user)=>
					user.FindFirstValue(ClaimTypes.NameIdentifier)!;
	
}
