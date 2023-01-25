using System.Text;
using AuthService.Common.Messages;
using AuthService.Common.Types;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AuthService.Common.RabbitMq;

public class BusPublisher : IBusPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BusPublisher> _logger;
    private IConnection _connection;
    private IModel _channel;

    public BusPublisher(IConfiguration configuration, ILogger<BusPublisher> logger, IOptions<RabbitMqOptions> options)
    {
        _configuration = configuration;
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = options.Value.Host,
            UserName = options.Value.Username,
            Password = options.Value.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _logger.LogInformation($"Connected to Message Bus: '{DateTime.UtcNow}'");
        _connection.ConnectionShutdown += ConnectionShutdown!;
    }

    public async Task SendAsync<TCommand>(TCommand command, ICorrelationContext context)
        where TCommand : ICommand
    {
        await Task.CompletedTask;
        TryHandleAsync(command, () =>
        {
            var body = Encoding.UTF8.GetBytes(command.Message);
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
            );
        });
    }

    public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context)
        where TEvent : IEvent
    {
        await Task.CompletedTask;
        TryHandleAsync(@event, () =>
        {
            var body = Encoding.UTF8.GetBytes(@event.Message);
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
            );
        });
    }

    private void TryHandleAsync<TIn>(TIn message, Action handle) where TIn : IMessage
    {
        try
        {
            handle();
            _logger.LogInformation($"Event published: '{message.UserId}'");
        }
        catch (Exception exception) when (exception is AuthException)
        {
            throw new AuthException(Codes.ServiceError, exception.Message);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }
    // private async Task

    private void ConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        if (!_channel.IsOpen) return;
        _channel.Close();
        _connection.Close();
        _logger.LogInformation("Connection stopped with RabbitMq");
    }
}