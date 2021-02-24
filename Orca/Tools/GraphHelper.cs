﻿using Microsoft.Graph;
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
using Microsoft.Extensions.Logging;

namespace Orca.Tools
{
    public class GraphHelper
    {
        private static GraphServiceClient _graphClient;
        private readonly string _appId;
        private readonly string _tenantId;
        private ILogger<GraphHelper> _logger;

        public GraphHelper(IOptions<MSGraphSettings> msGraphSettings, ILogger<GraphHelper> logger)
        {
            var settingsVal = msGraphSettings.Value;
            _appId = settingsVal.AppId;
            _tenantId = settingsVal.TenantId;
            string _clientSecret = settingsVal.ClientSecret;
            _logger = logger;

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

        public async Task<User> GetUserAsync(string userId)
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
                _logger.LogError($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<CallRecord> GetCallRecordSessions(string callId)
        {
            try
            {
                // GET /groups/{groupId}/Members
                return await _graphClient.Communications
                    .CallRecords[callId]
                    .Request()
                    .Expand("sessions")
                    .GetAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting call participants: {ex.Message}");
                return null;
            }
        }

        public async Task<CallRecord> GetCallRecord(string callId)
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
                _logger.LogError($"Error getting call participants: {ex.Message}");
                return null;
            }
        }

        public async Task<Subscription> CreateSubscription(String url)
        {
            var sub = new Microsoft.Graph.Subscription();
            sub.ChangeType = "updated";
            sub.NotificationUrl = url + "/api/notifications";
            sub.Resource = "/communications/callRecords";
            sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(4230);
            sub.ClientState = "SecretClientState";

            try
            {
                return await _graphClient
                    .Subscriptions
                    .Request()
                    .AddAsync(sub);
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error creating subscription: {ex.Message}");
                return null;
            }
        }

        public async void RenewSubscription(Subscription subscription)
        {
            Console.WriteLine($"Current subscription: {subscription.Id}, Expiration: {subscription.ExpirationDateTime}");

            var newSubscription = new Subscription
            {
                ExpirationDateTime = DateTime.UtcNow.AddMinutes(4230)
            };

            await _graphClient
              .Subscriptions[subscription.Id]
              .Request()
              .UpdateAsync(newSubscription);

            subscription.ExpirationDateTime = newSubscription.ExpirationDateTime;
            _logger.LogInformation($"Renewed subscription: {subscription.Id}, New Expiration: {subscription.ExpirationDateTime}");
        }
    }
}
