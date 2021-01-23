using Microsoft.Extensions.Logging;
using Orca.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        }
    }
}
