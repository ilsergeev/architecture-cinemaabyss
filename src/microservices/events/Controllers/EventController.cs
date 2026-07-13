using Events.Models;
using Microsoft.AspNetCore.Mvc;

namespace Events.Controllers
{
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IKafkaProducer _kafkaProducer;

        public EventController(ILogger<EventController> logger, IKafkaProducer kafkaProducer)
        {
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Создание события фильма
        /// </summary>
        /// <description>Регистрирует новое событие, связанное с фильмом</description>
        /// <response code="201"/>
        /// <response code="400"/>
        /// <response code="500"/>
        [HttpPost("movie")]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<IActionResult> CreateMovieEvent(MovieEvent movieEvent)
        {
            try
            {
                var deliveryResult = await _kafkaProducer.PublishEventAsync("movie-events", movieEvent);

                return StatusCode(201, new EventResponse
                {
                    Status = "success",
                    Partition = deliveryResult.Partition.Value,
                    Offset = (int)deliveryResult.Offset.Value,
                    Event = new Event
                    {
                        Id = $"movie-{movieEvent.MovieId}-{movieEvent.Action}",
                        Type = "movie",
                        Timestamp = DateTime.UtcNow,
                        Payload = movieEvent
                    },
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel { Error = ex.Message });
            }
        }

        /// <summary>
        /// Создание события пользователя
        /// </summary>
        /// <description>Регистрирует новое событие, связанное с пользователем</description>
        /// <response code="201"/>
        /// <response code="400"/>
        /// <response code="500"/>
        [HttpPost("user")]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<IActionResult> CreateUserEvent(UserEvent userEvent)
        {
            try
            {
                var deliveryResult = await _kafkaProducer.PublishEventAsync("user-events", userEvent);

                return StatusCode(201, new EventResponse
                {
                    Status = "success",
                    Partition = deliveryResult.Partition.Value,
                    Offset = (int)deliveryResult.Offset.Value,
                    Event = new Event
                    {
                        Id = $"user-{userEvent.UserId}-{userEvent.Action}",
                        Type = "user",
                        Timestamp = DateTime.UtcNow,
                        Payload = userEvent
                    },
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel { Error = ex.Message });
            }
        }

        /// <summary>
        /// Создание события платежа
        /// </summary>
        /// <description>Регистрирует новое событие, связанное с платежом</description>
        /// <response code="201"/>
        /// <response code="400"/>
        /// <response code="500"/>
        [HttpPost("payment")]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<IActionResult> CreatePaymentEvent(PaymentEvent paymentEvent)
        {
            try
            {
                var deliveryResult = await _kafkaProducer.PublishEventAsync("payment-events", paymentEvent);

                return StatusCode(201, new EventResponse
                {
                    Status = "success",
                    Partition = deliveryResult.Partition.Value,
                    Offset = (int)deliveryResult.Offset.Value,
                    Event = new Event
                    {
                        Id = $"payment-{paymentEvent.PaymentId}",
                        Type = "payment",
                        Timestamp = DateTime.UtcNow,
                        Payload = paymentEvent
                    },
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel { Error = ex.Message });
            }
        }

        /// <summary>
        /// Проверка работоспособности микросервиса событий
        /// </summary>
        /// <description>Возвращает статус работоспособности микросервиса событий</description>
        /// <response code="200"/>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public IActionResult GetEventsServiceHealth()
        {
            return Ok(new { status = true});
        }
    }
}
