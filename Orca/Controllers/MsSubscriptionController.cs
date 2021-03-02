using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Orca.Services;
using Orca.Services.MSGraphSubscription;
using Orca.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orca.Services.Adapters;
using Microsoft.Graph.CallRecords;
using Orca.Entities;
using EventType = Orca.Entities.EventType;

namespace Orca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly MSGraphSettings _config;
        private static Dictionary<string, Subscription> _subscriptions = new Dictionary<string, Subscription>();
        private GraphHelper _graphHelper;
        private ILogger<GraphHelper> _logger;
        private readonly MsGraphAdapter _graphAdapter;

        public NotificationsController(IOptions<MSGraphSettings> msGraphSettings, GraphHelper graphHelper, ILogger<GraphHelper> logger,MsGraphAdapter msGraphAdapter)
        {
            this._config = msGraphSettings.Value;
            this._graphHelper = graphHelper;
            this._logger = logger;
            _graphAdapter = msGraphAdapter;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromQuery] string validationToken = null)
        {
            // handle validation
            if (!string.IsNullOrEmpty(validationToken))
            {
                _logger.LogDebug($"Received Token: '{validationToken}'");
                return Ok(validationToken);
            }

            // handle notifications
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string content = await reader.ReadToEndAsync();

                _logger.LogDebug("Received notification: " + content);

                var notifications = JsonConvert.DeserializeObject<Notifications>(content);

                foreach (var notification in notifications.Items)
                {
                    _logger.LogDebug($"Received notification: '{notification.Resource}', {notification.ResourceData?.Id}");
                    var callRecord = await _graphHelper.GetCallRecordSessions(notification.ResourceData?.Id);
                    var joinWebUrl = (callRecord != null ? callRecord.JoinWebUrl : null);
                    if (joinWebUrl != null)
                    {
                        foreach (Session session in callRecord.Sessions)
                        {
                            ParticipantEndpoint caller = (ParticipantEndpoint)session.Caller;
                            var user = await _graphHelper.GetUserAsync(caller.Identity.User.Id);
                            if (user != null)
                            {
                                StudentEvent studentEvent = new StudentEvent
                                {
                                    //TODO - Find course ID based on joinWebUrl.
                                    CourseID = "COMP0088", // Course ID Upper case.
                                    Timestamp = ((DateTimeOffset)session.StartDateTime).UtcDateTime,
                                    EventType = EventType.Attendance,
                                    ActivityType = "Meeting",
                                    ActivityName = "Weekly Lecture",
                                    Student = new Student
                                    {
                                        Email = user.Mail,
                                        FirstName = user.GivenName,
                                        LastName = user.Surname,
                                        //TODO - Find the mapping between this userId and the university's student ID.
                                        ID = user.Id
                                    }
                                };
                                _logger.LogDebug("Student to be processed: " + studentEvent.ToString());
                                await _graphAdapter.ProcessEvents(studentEvent);
                            }
                        }
                    }
                    
                }
            }

            return Ok();
        }
    }
    }
