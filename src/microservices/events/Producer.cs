using Confluent.Kafka;
using System.Text.Json;

namespace Events
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, byte[]> _producer;

        public KafkaProducer(IProducer<Null, byte[]> producer)
        {
            _producer = producer;
        }

        public async Task<DeliveryResult<Null, byte[]>> PublishEventAsync<TEvent>(string topic, TEvent sendingEvent)
            where TEvent : class
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(sendingEvent);

            var result = await _producer.ProduceAsync(topic, new Message<Null, byte[]>
            {
                Value = payload,
            });

            return result;
        }
    }
}
