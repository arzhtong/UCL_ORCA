using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Entities;
using Orca.Services;
using OrcaTests.Tools;

namespace OrcaTests.Services
{
    public class EventAggregatorTests
    {
        // Previous function - DoNothingTest()
        [Fact]
        public async Task ProcessEventStoresStudentEventOnSharepoint()
        {
            var logger = new InMemoryLogger<EventAggregator>();
            var mockSharepointManager = new MockSharepointManager();
            mockSharepointManager.CreateList(EventAggregator.HARDCODED_LIST, "", new List<string>());
            var eventAggregator = new EventAggregator(mockSharepointManager, logger);
            
            await eventAggregator.ProcessEvent(new StudentEvent
            {
                CourseID = "cid",
                EventType = "Attendance",
                ActivityType = "Video",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            });
            var itemsInSharepointList = await mockSharepointManager.GetItemsFromList(EventAggregator.HARDCODED_LIST);
            
            Assert.Contains(itemsInSharepointList, item =>
            {
                string courseId = (string)item["CourseID"];
                return courseId == "cid";
            });
        }

    }

}
