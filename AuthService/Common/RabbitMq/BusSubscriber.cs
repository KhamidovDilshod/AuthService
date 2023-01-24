using System.Reflection.Metadata.Ecma335;
using AuthService.Common.Handlers;
using AuthService.Common.Messages;
using AuthService.Common.Types;
using OpenTracing;
using Polly;
using RawRabbit;
using RawRabbit.Common;

namespace AuthService.Common.RabbitMq;

public class BusSubscriber : IBusSubscriber
{
    private readonly ILogger<BusSubscriber> _logger;
    private readonly IBusClient _busClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITracer _tracer;
    private readonly int _retries;
    private readonly int _retryInterval;

    public BusSubscriber(IApplicationBuilder app)
    {
        _logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>() ??
                  throw new AuthException(Codes.ServiceError,
                      $"Error while getting service :'{nameof(ILogger<BusSubscriber>)}'");
        _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>() ??
                           throw new AuthException(Codes.ServiceError,
                               $"Error while getting service :'{nameof(IServiceProvider)}'");
        _busClient = _serviceProvider.GetService<IBusClient>() ??
                     throw new AuthException(Codes.ServiceError,
                         $"Error while getting service :'{nameof(IServiceProvider)}'");
        _tracer = _serviceProvider.GetService<ITracer>() ??
                  throw new AuthException(Codes.ServiceError,
                      $"Error while getting service :'{nameof(IServiceProvider)}'");
        var options = _serviceProvider.GetService<RabbitMqOptions>();
        _retries = options?.Retries > 3 ? options.Retries : 3;
        _retryInterval = options?.RetryInterval > 0 ? options.RetryInterval : 2;
    }

    public IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
        Func<TCommand, AuthException, IRejectedEvent> onError = null)
        where TCommand : ICommand
    {
        _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (comman, correlationContext) =>
        {
            var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            return await TryHandleAsync();
        });
    }

    public IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
        Func<TEvent, AuthException, IRejectedEvent> onError = null) where TEvent : IEvent
    {
        throw new NotImplementedException();
    }


    private async Task<Acknowledgement> TryHandleAsync<TMessage>(TMessage message,
        CorrelationContext correlationContext,
        Func<Task> handle, Func<TMessage, AuthException, IRejectedEvent> onError = null!)
    {
        var currentRetry = 0;
        var retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(_retries, i => TimeSpan.FromSeconds(_retryInterval));
        var messageName = message?.GetType().Name;

        return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
        {
            var scope = _tracer
                .BuildSpan("executing-handler")
                .AsChildOf(_tracer.ActiveSpan)
                .StartActive(true);

            using (scope)
            {
                var span = scope.Span;
                try
                {
                    var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}";
                    var preLogMessage = $"Handling a message: '{messageName}'" +
                                        $"with correlation id: '{correlationContext.Id}'.{retryMessage}";
                    _logger.LogInformation(preLogMessage);
                    span.Log(preLogMessage);

                    await handle();

                    var postLogMessage = $"Handle a message: '{messageName}' " +
                                         $"with correlation id: '{correlationContext.Id}'. '{retryMessage}'";
                    _logger.LogInformation(postLogMessage);
                    span.Log(postLogMessage);

                    return new Ack();
                }
                catch (Exception exception)
                {
                    throw;
                }
            }
        });
    }
}