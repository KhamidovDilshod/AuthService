﻿namespace AuthService.Common.Messages;

public interface IRejectedEvent:IEvent
{
    string Reason { get; }
    string Code { get; }
}