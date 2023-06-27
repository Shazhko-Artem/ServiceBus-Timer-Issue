using ServiceBus.Timer.Issue;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<ServiceBusConsumerOptions>(builder.Configuration.GetSection(ServiceBusConsumerOptions.Position));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHostedService<ServiceBusQueueConsumer>();
builder.Services.AddHostedService<ServiceBusTopicConsumer>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.Run();