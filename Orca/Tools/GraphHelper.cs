using Microsoft.Graph;
using Microsoft.Graph.CallRecords;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using System.Security;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;

namespace Orca.Tools
{
    public class GraphHelper
    {
        private static GraphServiceClient _graphClient;
        private readonly string _appId;
        private readonly string _tenantId;

        public GraphHelper(IOptions<MSGraphSettings> sharepointSettings)
        {
            var settingsVal = sharepointSettings.Value;
            _appId = settingsVal.AppId;
            _tenantId = settingsVal.TenantId;
            string _clientSecret = settingsVal.ClientSecret;

            // Initialize the auth provider with values from appsettings.json
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(_appId)
                .WithTenantId(_tenantId)
                .WithClientSecret(_clientSecret)
                .Build();

            //Install-Package Microsoft.Graph.Auth -PreRelease
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            _graphClient = new GraphServiceClient(authProvider);
        }

        public static async Task<User> GetUserAsync(string userId)
        {
            try
            {
                // GET /users/{id}
                return await _graphClient.Users[userId]
                    .Request()
                    .Select(e => new
                    {
                        e.Mail,
                        e.GivenName,
                        e.Surname,
                        e.Id
                    })
                    .GetAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public static async Task<ICallRecordSessionsCollectionPage> GetCallRecordSessions(string callId)
        {
            try
            {
                // GET /groups/{groupId}/Members
                return await _graphClient.Communications
                    .CallRecords[callId]
                    .Sessions
                    .Request()
                    .Expand("segments")
                    .GetAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting call participants: {ex.Message}");
                return null;
            }
        }

        public static async Task<CallRecord> GetCallRecord(string callId)
        {
            try
            {
                // GET /communications/callRecords/{callId}
                return await _graphClient.Communications
                    .CallRecords[callId]
                    .Request()
                    .GetAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting call participants: {ex.Message}");
                return null;
            }
        }
    }
}
