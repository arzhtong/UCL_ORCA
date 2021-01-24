using Microsoft.Extensions.Logging;
using Orca.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;
using Microsoft.SharePoint.Client;
using PnP.Framework;


namespace Orca.Services
{
    
    public class ConsoleEventAggregator : IEventAggregator
    {
        public ILogger<ConsoleEventAggregator> _logger;

        //the Azure app id, used for authentication
        private const string _azureAppId = "b269d983-e626-4d2d-bf17-606b0f2a93bb";
        private const string _sharepointUrl = "https://liveuclac.sharepoint.com/sites/ORCA";

        public ConsoleEventAggregator(ILogger<ConsoleEventAggregator> logger)
        {
            _logger = logger;
        }

        public void ProcessEvent(StudentEvent studentEvent)
        {
            _logger.LogInformation(studentEvent.ToString());
            // Starting with ClientContext, the constructor requires a URL to the server running SharePoint.
            StoreEventOnSharePoint(studentEvent);
        }

        public async void StoreEventOnSharePoint(StudentEvent studentEvent)
        {
            // Login Access Info, [[[Need to be changed before start up]]].
            // Please access the url below before start the program.
            // https://login.microsoftonline.com/1faf88fe-a998-4c5b-93c9-210a11d9a5c2/oauth2/v2.0/authorize?client_id=b269d983-e626-4d2d-bf17-606b0f2a93bb&scope=https://microsoft.sharepoint-df.com/AllSites.Manage&response_type=code
            string username = "username@ucl.ac.uk";
            string password = "your_password";

            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            // Secure the password.

            try
            {
                // Authentication.
                using (var authenticationManager = new PnP.Framework.AuthenticationManager(_azureAppId, username, securePassword))
                using (var context = authenticationManager.GetContext(_sharepointUrl))
                {
                    context.Load(context.Web, p => p.Title);
                    await context.ExecuteQueryAsync();
                    Console.WriteLine($"Title: {context.Web.Title}");
                    
                    Microsoft.SharePoint.Client.List eventList = context.Web.Lists.GetByTitle("Test Insertion");
                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                    ListItem eventItem = eventList.AddItem(itemInfo);
                    // Event Detailed Information.
                    eventItem["Title"] = "Event by " + studentEvent.Student.Email;
                    eventItem["CourseID"] = studentEvent.CourseID;
                    eventItem["StudentName"] = studentEvent.Student.FirstName + " (" + studentEvent.Student.LastName + ")";
                    eventItem["StudentID"] = studentEvent.Student.ID;
                    eventItem["EventType"] = studentEvent.EventType;
                    eventItem["ActivityType"] = studentEvent.ActivityType;
                    eventItem["Timestamp"] = studentEvent.Timestamp;

                    eventItem.Update();
                    context.ExecuteQuery();
                    Console.WriteLine("Event has been inserted into SharePoint successful.");

                }
            }
            catch (Exception e)
            {
                    Console.WriteLine("An Error Occurred.");
                    Console.WriteLine(e.Message);
            }
        }    
    }
}
