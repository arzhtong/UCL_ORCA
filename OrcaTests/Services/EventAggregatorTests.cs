using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrcaTests.Services;

namespace Orca.Services
{
    public class EventAggregatorTests
    {
        // Previous function - DoNothingTest()
        [Fact]
        public void ProcessEventLogsStudentEventAsString()
        {
            var logger = new InMemoryLogger<EventAggregator>();
            var eventAggregator = new EventAggregator(logger);

            eventAggregator.ProcessEvent(new Entities.StudentEvent
            {
                CourseID = "cid",
                EventType = "Attendance",
                ActivityType = "Video",
                Timestamp = DateTime.UtcNow,
                Student = new Entities.Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            });

            Assert.Contains("a.b@example.com", logger.LogLines[0]);
        }

    }

}
