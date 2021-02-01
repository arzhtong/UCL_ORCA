using Microsoft.Extensions.Logging;
using Orca.Entities;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services
{
    
    public class EventAggregator : IEventAggregator
    {
        public const string HARDCODED_LIST = "Test Insertion";
        private readonly ISharepointManager _sharePointManager;
        private ILogger<EventAggregator> _logger;

        public EventAggregator(ISharepointManager sharePointManager, ILogger<EventAggregator> logger)
        {
            _sharePointManager = sharePointManager;
            _logger = logger;
        }

        public async Task ProcessEvent(StudentEvent studentEvent)
        {
            Console.WriteLine("Event Aggregator has been called.");
            _logger.LogInformation(studentEvent.ToString());
            SharepointListItem eventItem = new SharepointListItem();
            // Event Detailed Information.
            eventItem["Title"] = "Event by " + studentEvent.Student.Email;
            eventItem["CourseID"] = studentEvent.CourseID;
            eventItem["StudentName"] = studentEvent.Student.FirstName + " (" + studentEvent.Student.LastName + ")";
            eventItem["StudentID"] = studentEvent.Student.ID;
            eventItem["EventType"] = studentEvent.EventType;
            eventItem["ActivityType"] = studentEvent.ActivityType;
            eventItem["Timestamp"] = studentEvent.Timestamp;

            await _sharePointManager.AddItemToList(HARDCODED_LIST, eventItem);
        }

    }
}
