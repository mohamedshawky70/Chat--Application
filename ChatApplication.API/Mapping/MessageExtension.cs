using ChatApplication.API.DTOs;

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
				message.Type.ToString(),
				SenderName: $"{message.Sender.FirstName} {message.Sender.LastName}"?? "Anonymous",
				message.ChatRoomId
			);
	}
	
	public static IEnumerable<MessageResponse> MapToMessageResponse(this IEnumerable<Message> messages)
	{
		return messages.Select(m =>m.MapToMessageResponse());
	}
}
