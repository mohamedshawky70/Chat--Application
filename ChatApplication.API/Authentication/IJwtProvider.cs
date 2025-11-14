namespace ChatApplication.API.Authentication;

public interface IJwtProvider
{
	(string taken, int expireIn) GenerateTaken(User user, IEnumerable<string> roles);

	//Refresh Taken
	string? ValidateTaken(string taken);
}
