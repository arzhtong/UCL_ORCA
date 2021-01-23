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

namespace Orca.Services
{
    public class ConsoleEventAggregator : IEventAggregator
    {
        public ILogger<ConsoleEventAggregator> _logger;

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

        public void StoreEventOnSharePoint(StudentEvent studentEvent)
        {
            // Login Access Info, need to change before start up.
            string username = "example@ucl.ac.uk";
            string password = "examplePassword";
            string url = "https://liveuclac.sharepoint.com/sites/ORCA";

            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }

            ClientContext clientContext = new ClientContext(url);
            Microsoft.SharePoint.Client.List myList = clientContext.Web.Lists.GetByTitle("events");
            ListItemCreationInformation itemInfo = new ListItemCreationInformation();
            ListItem myItem = myList.AddItem(itemInfo);
            myItem["Title"] = "Test Item";
            // myItem["Timestamp"] = DateTime.UtcNow;

            try
            {
                myItem.Update();
                var onlineCredentials = new SharePointOnlineCredentials(username, securePassword);
                clientContext.Credentials = onlineCredentials;
                clientContext.ExecuteQuery();
                Console.WriteLine("Successful.");

            }
            catch (Exception e)
            {
                Console.WriteLine("An Error Occurred.");
                Console.WriteLine(e.Message);
            }
        }

    }
}
