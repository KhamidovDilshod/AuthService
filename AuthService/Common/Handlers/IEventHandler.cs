using AuthService.Common.Messages;
using AuthService.Common.RabbitMq;

namespace AuthService.Common.Handlers;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, ICorrelationContext context);
}