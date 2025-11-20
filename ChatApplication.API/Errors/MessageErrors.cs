using Microsoft.AspNetCore.Http;

namespace ChatApplication.API.Errors;

public static class MessageErrors
{
	public readonly static Error NoMessagesFound = new("Messages.NoMessagesFound","No messages were found between the specified users.",StatusCodes.Status404NotFound);
	public readonly static Error NoMessagesUnread = new("Messages.NoMessagesUnread","No messages were found unread.",StatusCodes.Status404NotFound);
}
