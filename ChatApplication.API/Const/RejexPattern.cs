namespace ChatApplication.API.Const;

public class RejexPattern
{
	public const string StrongPassword = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
	public const string ValidphoneNumber = "^01[0,1,2,15]{1}[0-9]{8}$";
}
