using Confluent.Kafka;

namespace Events
{
    public interface IKafkaProducer
    {
        Task<DeliveryResult<Null, byte[]>> PublishEventAsync<TEvent>(string topic, TEvent sendingEvent) where TEvent : class;
    }
}