using ChatApplication.API.DTOs.File;
using ChatApplication.API.Mapping;

namespace ChatApplication.API.Services.FileService;

public class FileService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context) : IFileService
{
	private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
	private readonly ApplicationDbContext _context = context;
	private readonly string _filePath = $"{webHostEnvironment.WebRootPath}/Uploaded";
	public async Task<Result> UploadProfileAvatarAsync(UploadProfileAvatarRequest request, CancellationToken cancellationToken = default)
	{
		var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);


		string RootPath = _webHostEnvironment.WebRootPath; //...wwwroot
														   // Delete old avatar in state editing
		if (user.Avatar != null)
		{
			// maybe delete by mistake on server so should checking if it exists on server to avoid null exception
			var OldAvatar = Path.Combine($"{RootPath}/Avatars", user.Avatar);
			if (File.Exists(OldAvatar))
				File.Delete(OldAvatar);
		}

		var Extension = Path.GetExtension(request.Avatar?.FileName); //.jpg, .jpeg, .png

		var ImageName = $"{Guid.NewGuid()}{Extension}";  //[random name] /3456sd23rf.png(generate GUID To be uninq in db) 
		string ImgPath = Path.Combine($"{RootPath}/Avatars", ImageName); //  wwwroot/Avatar/rt4wfj.png
		using var stream = File.Create(ImgPath);// Make this path to bits to set in it the image 
		await request.Avatar!.CopyToAsync(stream, cancellationToken);// set in it the image [asyc for OS]
		user.Avatar = ImageName;// URl in db
		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}

	public async Task<Result<Guid>> UploadFileAsync(UploadFileRquest request, int messageId, CancellationToken cancellationToken = default)
	{
		var randomFileName = Path.GetRandomFileName();//Fake name with fake extension because if anyone reach these files on server

		var uploadFile = request.MapToUploadFile();
		uploadFile.StoredFileName = randomFileName;
		uploadFile.MessageId = messageId;

		//Save on server
		var path = Path.Combine(_filePath, randomFileName);//string1+string2
		using var stream = File.Create(path);//حولي الباث ده لبيتس (فايل) علشان اعرف استقبل فيه الفايل
		await request.File.CopyToAsync(stream, cancellationToken);// إستقبل الفايل اللي جاي من اليوزر في المكان ده

		// Save on database
		await _context.UploadFiles.AddAsync(uploadFile, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success(uploadFile.Id);
	}

	public async Task<(byte[] fileContent, string ContentType, string fileName)> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var file = await _context.UploadFiles.FindAsync(id, cancellationToken);
		if (file is null)
			return ([], string.Empty, string.Empty);
		var path = Path.Combine(_filePath, file.StoredFileName);
		MemoryStream memoryStream = new();
		using FileStream fileStream = new(path, FileMode.Open);// اقري اللي في الباث ده
		fileStream.CopyTo(memoryStream);

		memoryStream.Position = 0;

		return (memoryStream.ToArray(), file.ContentType, file.FileName);

	}
}
