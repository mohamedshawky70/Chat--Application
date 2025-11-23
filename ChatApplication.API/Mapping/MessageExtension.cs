using ChatApplication.API.DTOs.Message;

namespace ChatApplication.API.Mapping;

public static class MessageExtension
{
	public static MessageResponse MapToMessageResponse(this Message message)
	{
		return new MessageResponse
			(
				message.Content,
				message.SenderId,
				message.ReceiverId,
				message.SentAt,
				message.IsRead,
				message.IsPinned,
				PinnedBy :message.IsPinned? message.PinnedId==message.SenderId ? $"{message.Sender?.FirstName} {message.Sender?.LastName}":
										   $"{message.Receiver?.FirstName} {message.Receiver?.LastName}":"",
				message.Type.ToString(),
				SenderName: $"{message.Sender?.FirstName} {message.Sender?.LastName}"?? "Anonymous",
				message.ChatRoomId
			);
	}
	
	public static IEnumerable<MessageResponse> MapToMessageResponse(this IEnumerable<Message> messages)
	{
		return messages.Select(m =>m.MapToMessageResponse());
	}

	public static Message MapToMessage(this SendMessageRequest request)
	{
		return new Message
		{
			Content = request.Content,
			SenderId = request.SenderId,
			ReceiverId = request.ReceiverId,
			ChatRoomId = request.ChatRoomId,
			SentAt = DateTime.UtcNow
		};
	}
}
