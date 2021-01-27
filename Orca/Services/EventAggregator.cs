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
        private ILogger<EventAggregator> _logger;

        public EventAggregator(ILogger<EventAggregator> logger)
        {
            _logger = logger;
        }

        public void ProcessEvent(StudentEvent studentEvent)
        {
            Console.WriteLine("Event Aggregator has been called.");
            _logger.LogInformation(studentEvent.ToString());
            // Starting with ClientContext, the constructor requires a URL to the server running SharePoint.
            SharePointEventHandler.StoreEventOnSharePoint(studentEvent);
        }

    }
}
