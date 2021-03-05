using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Orca.Services
{
    /// <summary>
    /// Stores various pieces of information needed to connect to Microsoft Graph 
    /// </summary>
    public class MSGraphSettings
    {
        /// 
        /// <summary>
        /// The App ID registered on Azure AD
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// The Tenant ID registered on Azure AD
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The ClientSecret registered on Azure AD
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Ngrok
        /// </summary>
        public string Domain { get; set; }

    }
}
