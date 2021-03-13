using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services
{
    public class MsGraphIdentityResolver : IIdentityResolver
    {
        private readonly IGraphHelper _graphHelper;

        public MsGraphIdentityResolver(IGraphHelper graphHelper)
        {
            _graphHelper = graphHelper;
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            return await _graphHelper.GetUserIdByMailAsync(email);
        }
    }
}
