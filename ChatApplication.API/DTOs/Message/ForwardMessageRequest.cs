namespace ChatApplication.API.DTOs.Message;

public record ForwardMessageRequest
(
    List<string>? ForwardToUserIds = null,
    List<int>? ForwardToRoomIds = null,
    string? Caption = null // optional 
);                          
