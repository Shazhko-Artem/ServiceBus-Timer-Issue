using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace ServiceBus.Timer.Issue;

public class ServiceBusQueueConsumer : BackgroundService
{
    private const string QueueName = "myQueue";
    private readonly ServiceBusConsumerOptions _options;
    private readonly ServiceBusProcessor _processor;

    public ServiceBusQueueConsumer(IOptions<ServiceBusConsumerOptions> options)
    {
        _options = options.Value;
        var client = new ServiceBusClient(_options.ConnectionString, new ServiceBusClientOptions()
        {
            RetryOptions = new ServiceBusRetryOptions()
            {
                TryTimeout = TimeSpan.FromHours(1)
            }
        });
        _processor = client.CreateProcessor(QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping the receiver...");
        await _processor.StopProcessingAsync(cancellationToken);
        Console.WriteLine("Stopped receiving messages");

        await _processor.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}