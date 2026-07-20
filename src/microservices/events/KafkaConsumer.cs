using Confluent.Kafka;
using System.Text;

namespace Events
{
    public class KafkaConsumer : BackgroundService
    {
        private IConsumer<Null, byte[]> _consumer;
        private readonly static string[] _topics = new[] { "user-events", "payment-events", "movie-events" };
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(IConsumer<Null, byte[]> consumer, ILogger<KafkaConsumer> logger)
        {
            _consumer = consumer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    _consumer.Subscribe(_topics);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {

                            var result = _consumer.Consume(cancellationToken);
                            var message = Encoding.UTF8.GetString(result.Message.Value);
                            _logger.LogInformation("Consumed from {topic}: {value}", result.Topic, message);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogWarning(ex, "Consume error, retrying");
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (OperationCanceledException) { }
                finally
                {
                    _consumer.Close();
                    _consumer.Dispose();
                }
            }, cancellationToken);

        }
    }
}
