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

        // Attributes of default event list.
        public const string EVENT_LIST_COURSE_ID = "CourseId";
        public const string EVENT_LIST_STUDENT_NAME = "StudentName";
        public const string EVENT_LIST_STUDENT_EMAIL = "StudentEmail";
        public const string EVENT_LIST_STUDENT_ID = "StudentId";
        public const string EVENT_LIST_ACTIVITY_TYPE = "ActivityType";
        public const string EVENT_LIST_ACTIVITY_NAME = "ActivityName";
        public const string EVENT_LIST_TIMESTAMP = "Timestamp";

        public EventAggregator(ISharepointManager sharePointManager, ICourseCatalog courseCatalog, ILogger<EventAggregator> logger)
        {
            _sharePointManager = sharePointManager;
            _courseCatalog = courseCatalog;
            _logger = logger;
        }

        public async Task ProcessEvent(StudentEvent studentEvent)
        {


            // Check whether courseId exist in the courseCatalog.
            if (_courseCatalog.CheckCourseIdExist(studentEvent.CourseID))
            {
                // Only Attendance event will be sent to ORCA SharePoint.
                if (studentEvent.EventType == EventType.Attendance)
                {
                    // Check the corresponding list of this course according to Catalog.
                    string targetList = _courseCatalog.GetListNameForCourse(studentEvent.CourseID);
                    _logger.LogInformation("Event aggregator will send event to list \"{0}\".", targetList);
                    
                    // Check whether the target list exist in ORCA SharePoint.
                    // Once the list has been created successful, do the next step.
                    while (!_sharePointManager.CheckListExists(targetList))
                    {
                        _logger.LogInformation("Currently \"{0}\" does not exist, event aggregator will create the list.", targetList);
                        // If not exist, create new list.
                        string description = "This list is for store " + targetList + ".";
                        List<string> list = CreateDefaultSharePointEventListSchema();
                        _sharePointManager.CreateList(targetList, description, list);
                    }
                    _logger.LogInformation("List \"{0}\" is now exist and ready to store events.", targetList);
                    // Assign to different list by course ID.
                    SharepointListItem eventItem = PackEventItem(studentEvent);
                    await _sharePointManager.AddItemToList(targetList, eventItem);
                }

                // All events will then be stored in database.
                StoreEventInDatabase(studentEvent);
            }
            else
            {
                _logger.LogInformation($"Cannot find the courseId '{studentEvent.CourseID}', event aggregator has cancelled current event.");
            }
        }

        private List<string> CreateDefaultSharePointEventListSchema()
        {
            // Default schema of a list that store events.
            // This function is temporarily exist here, might be moved or deleted.
            List<string> eventList = new List<string>
            {
                $"<Field DisplayName='{EVENT_LIST_COURSE_ID}' Type='Text' />",
                $"<Field DisplayName='{EVENT_LIST_STUDENT_NAME}' Type='Text' Required='TRUE' />",
                $"<Field DisplayName='{EVENT_LIST_STUDENT_EMAIL}' Type='Text' Required='TRUE' />",
                $"<Field DisplayName='{EVENT_LIST_STUDENT_ID}' Type='Text' Required='TRUE' />",
                $"<Field DisplayName='{EVENT_LIST_ACTIVITY_TYPE}' Type='Text' />",
                $"<Field DisplayName='{EVENT_LIST_ACTIVITY_NAME}' Type='Text' />",
                $"<Field DisplayName='{EVENT_LIST_TIMESTAMP}' Type='DateTime' Required='TRUE' />"
            };

            return eventList;
        }

        private SharepointListItem PackEventItem(StudentEvent studentEvent)
        {
            SharepointListItem eventItem = new SharepointListItem();
            // Event Detailed Information.
            eventItem[EVENT_LIST_COURSE_ID] = studentEvent.CourseID.ToUpper();
            eventItem[EVENT_LIST_STUDENT_NAME] = studentEvent.Student.LastName + ", " + studentEvent.Student.FirstName;
            eventItem[EVENT_LIST_STUDENT_EMAIL] = studentEvent.Student.Email;
            eventItem[EVENT_LIST_STUDENT_ID] = studentEvent.Student.ID;
            eventItem[EVENT_LIST_ACTIVITY_TYPE] = studentEvent.ActivityType;
            eventItem[EVENT_LIST_ACTIVITY_NAME] = studentEvent.ActivityName;
            eventItem[EVENT_LIST_TIMESTAMP] = studentEvent.Timestamp;

            _logger.LogInformation("Event by {0} has been packed.", studentEvent.Student.FirstName);
            return eventItem;
        }

        private void StoreEventInDatabase(StudentEvent studentEvent)
        {
            // Need to be implemented.
            _logger.LogInformation("New Student Event:\n" + studentEvent.ToString());
        }
    }
}
