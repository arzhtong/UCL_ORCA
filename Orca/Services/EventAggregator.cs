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
        
        private readonly ISharepointManager _sharePointManager;
        private readonly ICourseCatalog _courseCatalog;
        private ILogger<EventAggregator> _logger;
        public EventAggregator(ISharepointManager sharePointManager, ICourseCatalog courseCatalog, ILogger<EventAggregator> logger)
        {
            _sharePointManager = sharePointManager;
            _courseCatalog = courseCatalog;
            _logger = logger;
        }

        public async Task ProcessEvent(StudentEvent studentEvent)
        {
            Console.WriteLine("Event aggregator has received a new event.");
            _logger.LogInformation(studentEvent.ToString());
            
            // Check the corresponding list of this course according to Catalog.
            string targetList;
            try 
            {
                //Check courseId exist in the coursecatalog
                if (_courseCatalog.CheckCourseIdExist(studentEvent.CourseID))
                {
                    targetList = _courseCatalog.GetListNameForCourse(studentEvent.CourseID);
                    Console.WriteLine("Event aggregator will send event to list {0}.", targetList);

                    SharepointListItem eventItem = new SharepointListItem();
                    // Event Detailed Information.
                    eventItem["Title"] = "Event by " + studentEvent.Student.Email;
                    eventItem["CourseID"] = studentEvent.CourseID.ToUpper();
                    eventItem["StudentName"] = studentEvent.Student.FirstName + " (" + studentEvent.Student.LastName + ")";
                    eventItem["StudentID"] = studentEvent.Student.ID;
                    eventItem["EventType"] = studentEvent.EventType;
                    eventItem["ActivityType"] = studentEvent.ActivityType;
                    eventItem["Timestamp"] = studentEvent.Timestamp;

                    // Assign to different list by course ID.
                    await _sharePointManager.AddItemToList(targetList, eventItem);
                }
                else
                {
                    Console.WriteLine("courseId is not in the courseCatalog");
                    _logger.LogInformation("Cannot find the courseId, event aggregator has cancelled current event.");
                }
            }
            catch (KeyNotFoundException e)
            {
                // Cannot find this course in Catalog.
                Console.WriteLine("List is not found.");
                _logger.LogInformation("Cannot find the list, event aggregator has cancelled current event.");
                _logger.LogError(e.Message);
            }
        }
    }
}
