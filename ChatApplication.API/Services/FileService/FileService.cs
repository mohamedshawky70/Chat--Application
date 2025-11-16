using ChatApplication.API.DTOs.File;

namespace ChatApplication.API.Services.FileService;

public class FileService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context) : IFileService
{
	private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
	private readonly ApplicationDbContext _context = context;

	public async Task<Result> UploadProfileAvatarAsync(UploadProfileAvatarRequest request, CancellationToken cancellationToken = default)
	{
		var user = await _context.Users.FindAsync(request.UserId);
		if (user == null)
			return Result.Failure(UserError.UserNotFound);

		var Extension = Path.GetExtension(request.Avatar?.FileName); //.jpg, .jpeg, .png
		string RootPath = _webHostEnvironment.WebRootPath; //...wwwroot
		var ImageName = $"{Guid.NewGuid()}{Extension}";  //[random name] /3456sd23rf.png(generate GUID To be uninq in db) 
		string ImgPath = Path.Combine($"{RootPath}/Avatars", ImageName); //  wwwroot\Images\Book\rt4wfj.png
		using var stream = File.Create(ImgPath);//حولي الباث ده لبيتس علشان اعرف استقبل فيه صوره 
		await request.Avatar!.CopyToAsync(stream);// (هنا بكلم الاوبريتنج سيستم يبقا يفضل Async)// إستقبل فيه الصورة
		user.Avatar = ImageName;// قيمه في الداتابيز 
		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
