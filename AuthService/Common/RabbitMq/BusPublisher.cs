using AuthService.Common.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace AuthService.Common.RabbitMq;

public class BusPublisher : IBusPublisher
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public BusPublisher(IConfiguration configuration, IOptions<RabbitMqOptions> options)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory
        {
            HostName = options.Value.Host,
            UserName = options.Value.Username,
            Password = options.Value.Password
        };
    }

    public async Task SendAsync<TCommand>(TCommand command, ICorrelationContext context)
        where TCommand : ICommand
        => await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context));

    public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context)
        where TEvent : IEvent
        => await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context));
}