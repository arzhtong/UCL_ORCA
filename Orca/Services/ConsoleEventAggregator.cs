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

//access the url below
//https://login.microsoftonline.com/1faf88fe-a998-4c5b-93c9-210a11d9a5c2/oauth2/v2.0/authorize?client_id=b269d983-e626-4d2d-bf17-606b0f2a93bb&scope=https://microsoft.sharepoint-df.com/AllSites.Manage&response_type=code
namespace Orca.Services
{
    
    public class ConsoleEventAggregator : IEventAggregator
    {
        //the Azure app id, used for authentication
        private const string _azureAppId = "b269d983-e626-4d2d-bf17-606b0f2a93bb";
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

        public async void StoreEventOnSharePoint(StudentEvent studentEvent)
        {
            // Login Access Info, need to change before start up.
            string username = "username@ucl.ac.uk";
            string password = "password";
            string url = "https://liveuclac.sharepoint.com/sites/ORCA";
            //string azureAppId = "b269d983-e626-4d2d-bf17-606b0f2a93bb";
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            using (var authenticationManager = new PnP.Framework.AuthenticationManager(_azureAppId, username, securePassword))
            using (var context = authenticationManager.GetContext(url))
            {
                context.Load(context.Web, p => p.Title);
                await context.ExecuteQueryAsync();
                Console.WriteLine($"Title: {context.Web.Title}");
                
                Microsoft.SharePoint.Client.List myList = context.Web.Lists.GetByTitle("Test Insertion");
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                ListItem myItem = myList.AddItem(itemInfo);
                myItem["Title"] = "Test Item2";
                myItem["Timestamp"] = DateTime.UtcNow;
                Console.WriteLine(myList);
                try
                {
                    myItem.Update();
                    context.ExecuteQuery();
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
}
