using System.Text;
using AuthService.Common.Messages;
using AuthService.Common.Types;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace AuthService.Common.RabbitMq;

public class BusSubscriber : BackgroundService
{
    // private readonly IBusClient _busClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _retries;
    private readonly int _retryInterval;

    private readonly ILogger<BusSubscriber> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _queueName;


    public BusSubscriber(IApplicationBuilder app)
    {
        _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>() ??
                           throw new AuthException($"Couldn't get service: '{nameof(IServiceProvider)}'");
        _logger = _serviceProvider.GetService<ILogger<BusSubscriber>>() ??
                  throw new AuthException($"Couldn't get service: '{nameof(ILogger<BusSubscriber>)}'");
        // _busClient = _serviceProvider.GetService<IBusClient>() ??
        //              throw new AuthException($"Couldn't get service: '{nameof(IBusClient)}'");
        var options = _serviceProvider.GetService<RabbitMqOptions>() ??
                      throw new AuthException($"Couldn't get service: '{nameof(RabbitMqOptions)}'");
        ConnectToMessageBus(options);
        _retries = options.Retries >= 0 ? options.Retries : 3;
        _retryInterval = options.RetryInterval >= 0 ? options.RetryInterval : 2;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
    
        var consumer = new EventingBasicConsumer(_channel);
    
        consumer.Received += (moduleHandle, eventArgs) =>
        {
            Console.WriteLine("Event received");
            var body = eventArgs.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public BackgroundService SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
        Func<TCommand, AuthException, IRejectedEvent> onError = null)
        where TCommand : ICommand
    {
        // _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
        // {
        //     var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
        //
        //     return await TryHandleAsync(command, correlationContext,
        //         () => commandHandler!.HandleAsync(command, correlationContext), onError);
        // });
        return this;
    }

    public BackgroundService SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
        Func<TEvent, AuthException, IRejectedEvent> onError = null!)
        where TEvent : IEvent
    {
        // _busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, correlationContext) =>
        // {
        //     var eventHandler = _serviceProvider.GetService<IEventHandler<TEvent>>();
        //
        //     return await TryHandleAsync(@event, correlationContext,
        //         () => eventHandler!.HandleAsync(@event, correlationContext), onError);
        // });
        return this;
    }


    private async Task TryHandleAsync<TMessage>(TMessage message,
        CorrelationContext correlationContext,
        Func<Task> handle, Func<TMessage, AuthException, IRejectedEvent> onError = null!)
    {
        var currentRetry = 0;
        var messageName = message?.GetType().Name;

        // return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
        // {
        //     try
        //     {
        //         var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}";
        //         var preLogMessage = $"Handling a message: '{messageName}'" +
        //                             $"with correlation id: '{correlationContext.Id}'.{retryMessage}";
        //         _logger.LogInformation(preLogMessage);
        //         await handle();
        //         var postLogMessage = $"Handle a message: '{messageName}' " +
        //                              $"with correlation id: '{correlationContext.Id}'. '{retryMessage}'";
        //         _logger.LogInformation(postLogMessage);
        //         return new Ack();
        //     }
        //     catch (Exception exception)
        //     {
        //         currentRetry++;
        //         _logger.LogError(exception, exception.Message);
        //         if (exception is AuthException authException && onError != null)
        //         {
        //             var rejectedEvent = onError(message, authException);
        //             await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));
        //             _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}'" +
        //                                    $"for the message: '{messageName}' with correlation id : '{correlationContext.Id}'.");
        //             return new Ack();
        //         }
        //
        //         throw new AuthException($"Unable to handle a message : '{messageName}'" +
        //                                 $"with correlation id: '{correlationContext.Id}', " +
        //                                 $"retry {currentRetry - 1}/{_retries}...");
        //     }
        // });
        await Task.CompletedTask;
    }

    private void ConnectionShutdown(object sender, ShutdownEventArgs args) =>
        Console.WriteLine("Connection closed {0}", DateTime.UtcNow);

    private void ConnectToMessageBus(RabbitMqOptions options)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = options.Host,
            UserName = options.Username,
            Password = options.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
        _connection.ConnectionShutdown += ConnectionShutdown!;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen) _connection.Close();
        base.Dispose();
    }
}