using AuthService.Common.Messages;
using AuthService.Common.Types;

namespace AuthService.Common.RabbitMq;

public interface IBusSubscriber
{
    IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
        Func<TCommand, AuthException, IRejectedEvent> onError = null)
        where TCommand : ICommand;

    IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
        Func<TEvent, AuthException, IRejectedEvent> onError = null)
        where TEvent : IEvent;
}