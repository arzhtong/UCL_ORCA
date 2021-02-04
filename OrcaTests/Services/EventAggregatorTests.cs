using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Entities;
using Orca.Services;
using OrcaTests.Tools;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace OrcaTests.Services
{
    public class EventAggregatorTests
    {
        // Previous function - DoNothingTest()
        [Fact]
        public async Task ProcessEventStoresStudentEventOnSharepoint()
        {
            var eventAggregatorLogger = new InMemoryLogger<EventAggregator>();
            var catalogLogger = new InMemoryLogger<MockSharepointCourseCatalog>();
            var mockSharepointManager = new MockSharepointManager();
            var settings = SharepointCourseCatalogTests.SharepointSettingsWithCourseCatalogName("CourseCatalog");
            var courseCatalog = new MockSharepointCourseCatalog(Options.Create(settings), catalogLogger, mockSharepointManager);
            mockSharepointManager.CreateList("Test Events 1", "", new List<string>());
            var eventAggregator = new EventAggregator(mockSharepointManager, courseCatalog, eventAggregatorLogger);

            await eventAggregator.ProcessEvent(new StudentEvent
            {
                CourseID = "COMP0101",
                EventType = "Attendance",
                ActivityType = "Video",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            });
            mockSharepointManager.CheckListExists("Test Events 1");
            var list = courseCatalog.GetListNameForCourse("COMP0101");
            var itemsInSharepointList = await mockSharepointManager.GetItemsFromList(list);
            Assert.Equal("Test Events 1", list);
            Assert.Contains(itemsInSharepointList, item =>
            {
                string courseId = (string)item["CourseID"];
                return courseId == "COMP0101";
            });


            await eventAggregator.ProcessEvent(new StudentEvent
            {
                CourseID = "COMP0100",
                EventType = "Attendance",
                ActivityType = "Video",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            });
            Assert.Throws<KeyNotFoundException>(() => courseCatalog.GetListNameForCourse("COMP0100"));
        }
    }
}
