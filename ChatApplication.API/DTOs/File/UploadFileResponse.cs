using ChatApplication.API.enums;

namespace ChatApplication.API.DTOs.File;

public record UploadFileResponse
(
	 int MessageId, 
     string AttachmentUrl ,
     string FileName ,
     string ContentType 
);
