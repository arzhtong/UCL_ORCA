using Orca.Services;
using Microsoft.Extensions.Logging;
using Orca.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Orca.Controllers
{
    // -- /api/events
    [ApiController]
    [Route("api/events")]
    public class ConsoleEventController : ControllerBase
    {
        private readonly IEventAggregator _eventAggregator;
        // IEventAggregator is the interface of console event aggregator.

        public ConsoleEventController(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            // Passing event aggregator to console event controller.
        }

        [HttpGet]
        public StudentEvent GenerateEvent()
        {
            StudentEvent testEvent = new StudentEvent
            {
                CourseID = "comp0001",
                Timestamp = DateTime.UtcNow,
                EventType = "Engagement",
                ActivityType = "Video",
                Student = new Student { Email = "david.jackson@example.com", FirstName = "david", LastName = "jackson", ID = "00001" }
            };

            _eventAggregator.ProcessEvent(testEvent);
            // Print event info in console.
            return testEvent;
            // Get response on the swagger UI.
        }
    }
}
