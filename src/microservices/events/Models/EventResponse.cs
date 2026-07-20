namespace Events.Models
{
    public class EventResponse
    {
        public string Status { get; set; } = null!;
        public int Partition { get; set; }
        public int Offset { get; set; }
        public Event Event { get; set; } = null!;
    }
}
