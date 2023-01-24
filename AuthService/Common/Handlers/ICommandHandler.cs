using AuthService.Common.Messages;
using AuthService.Common.RabbitMq;

namespace AuthService.Common.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, ICorrelationContext context);
}