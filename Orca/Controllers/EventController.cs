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
    public class EventController : ControllerBase
    {
        private readonly IEventAggregator _eventAggregator;
        // IEventAggregator is the interface of console event aggregator.

        public EventController(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            // Passing event aggregator to console event controller.
        }

        [HttpGet]
        public StudentEvent GenerateEvent()
        {
            StudentEvent testEvent = new StudentEvent
            {
                CourseID = "comp0102",
                Timestamp = DateTime.UtcNow,
                EventType = "Engagement",
                ActivityType = "Quiz",
                Student = new Student { Email = "tom.jerry@example.com", FirstName = "Tom", LastName = "Jerry", ID = "202056789" }
            };

            _eventAggregator.ProcessEvent(testEvent);
            // Print event info in console.
            return testEvent;
            // Get response on the swagger UI.
        }

    }
}
