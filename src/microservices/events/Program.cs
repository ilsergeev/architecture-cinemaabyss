using Confluent.Kafka;
using Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var kafkaProducerConfig = new ProducerConfig
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"]
    };

    return new ProducerBuilder<Null, byte[]>(kafkaProducerConfig).Build();
});

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var kafkaConsumerConfig = new ConsumerConfig
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"],
        GroupId = configuration["Kafka:GroupId"],
        AutoOffsetReset = AutoOffsetReset.Earliest,
    };

    return new ConsumerBuilder<Null, byte[]>(kafkaConsumerConfig).Build();
});

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<KafkaConsumer>();


var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
