using Microsoft.Graph.CallRecords;
using Orca.Entities;
using Orca.Entities.Dtos;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services.Adapters
{
    public class MsGraphAdapter
    {
        private readonly IEventAggregator _eventAggregator;

        public MsGraphAdapter(IEventAggregator eventAggregator, GraphHelper graphHelper)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task ProcessEvents(StudentEvent studentEvent)
        {

            await _eventAggregator.ProcessEvent(studentEvent);
          
        }

    }
}
