using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orca.Entities;
using Orca.Tools;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Orca.Scheduling
{
    /// <summary>
    /// A scheduler used for creating an MS Graph Webhook subscription for /communications/callRecord and updating it.
    /// </summary>
    public class MsGraphSubscriptionUpdater : BackgroundService
    {
        private const int _dELAY_TIME_MS = 5 * 60 * 1000;
        private const int _subscriptionMinutes = 15;
        private readonly ILogger<MsGraphSubscriptionUpdater> _logger;
        private GraphHelper _graphHelper;
        private readonly MSGraphSettings _config;
        private static Dictionary<string, Subscription> _subscriptions = new Dictionary<string, Subscription>();

        public MsGraphSubscriptionUpdater(IOptions<MSGraphSettings> msGraphSettings,ILogger<MsGraphSubscriptionUpdater> logger, GraphHelper graphHelper)
        {
            this._config = msGraphSettings.Value;
            _logger = logger;
            _graphHelper = graphHelper;
        }
        // Executes the scheduler in the background. It uses a cancellation token to stop the scheduler at a fixed time period
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"MsGraphSubscriptionUpdater is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($" MsGraphSubscriptionUpdater background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Updating MsGraph subscription.");
                await CheckSubscriptionsAsync();

                await Task.Delay(_dELAY_TIME_MS, stoppingToken);
            }

            _logger.LogInformation($"MsGraphSubscriptionUpdater background task is stopping.");
        }

        private async Task CheckSubscriptionsAsync()
        {

            _logger.LogDebug($"Checking subscriptions {DateTime.Now.ToString("h:mm:ss.fff")}");
            _logger.LogDebug($"Current subscription count {_subscriptions.Count()}");

            if (_subscriptions.Count() == 0)
            {
                var newSubscription = await _graphHelper.CreateSubscription(_config.Ngrok, _subscriptionMinutes);
                if( newSubscription != null)
                {
                    _subscriptions[newSubscription.Id] = newSubscription;
                }
            }
            else
            {
                foreach (var subscription in _subscriptions)
                {
                    // if the subscription expires in the next 5 min, renew it
                    if (subscription.Value.ExpirationDateTime < DateTime.UtcNow.AddMinutes(5))
                    {
                        _graphHelper.RenewSubscription(subscription.Value, _subscriptionMinutes);
                    }
                }
            }

        }

    }
}
