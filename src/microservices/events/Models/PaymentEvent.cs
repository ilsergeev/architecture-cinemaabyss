using System.Text.Json.Serialization;

namespace Events.Models
{
    public class PaymentEvent
    {
        [JsonPropertyName("payment_id")]
        public int PaymentId { get; set; }
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("method_type")]
        public string? MethodType { get; set; }
    }
}
