namespace ChatApplication.API.Entites;

public class UploadFile
{
	public Guid Id { get; set; } = Guid.CreateVersion7();
	public string FileName { get; set; } = default!;
	public string StoredFileName { get; set; } = default!;
	public string ContentType { get; set; } = default!;
	public string FileExtension { get; set; } = default!;
	public int MessageId { get; set; }                     
    public Message Message { get; set; } = null!;
}
