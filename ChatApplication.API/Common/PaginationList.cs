namespace ChatApplication.API.Common;

public class PaginationList<T>
{
	public PaginationList(List<T> items, int pageNumber, int count, int pageSize)
	{
		Items = items;
		PageNumber = pageNumber;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
	}
	public List<T> Items { get; private set; }
	public int PageNumber { get; private set; }
	public int TotalPages { get; private set; }
	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;
	                                                                     
	public static async Task<PaginationList<T>> CreateAsync(IEnumerable <T> source, int pageNumber, int pageSize)
	{
		var count =  source.Count();
		var items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		return new PaginationList<T>(items, pageNumber, count, pageSize);
	}
}
