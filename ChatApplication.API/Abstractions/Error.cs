namespace ChatApplication.API.Abstractions;

//Automatically generates constructor, properties, ToString(), Equals(), and GetHashCode().
public record Error(string Code, string Description,int? StatusCode)
{
	public static readonly Error None = new Error(string.Empty, string.Empty,null);
}

//The same above record can be written as class as below
//public class Error
//{
//	public string Code { get; set; }
//	public string Description { get; set; }
//	public Error(string Code,string Description)
//	{
//		this.Code=Code;
//		this.Description=Description;
//	}

//   public static readonly Error None=new Error(string.Empty,string.Empty); 

//}
