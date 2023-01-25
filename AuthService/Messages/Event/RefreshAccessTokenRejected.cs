﻿using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class RefreshAccessTokenRejected : IRejectedEvent
{
    public string Message { get; set; }
    public Guid UserId { get; }
    public string Reason { get; }
    public string Code { get; }
    
    [JsonConstructor]
    public RefreshAccessTokenRejected(Guid userId, string reason, string code, string message)
    {
        UserId = userId;
        Reason = reason;
        Code = code;
        Message = message;
    }
}