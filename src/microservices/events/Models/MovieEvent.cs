using System.Text.Json.Serialization;

namespace Events.Models
{
    public class MovieEvent
    {
        [JsonPropertyName("movie_id")]
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string Action { get; set; } = null!;
        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }
        public float? Rating { get; set; }
        public IEnumerable<string>? Genres { get; set; }
        public string? Description { get; set; }
    }
}
