using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Orca.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Orca.Entities;

namespace Orca.Tools
{
    public class SharepointManager : IDisposable, ISharepointManager
    {
        private readonly string _azureAppId;
        private readonly string _sharepointUrl;
        private bool _disposedValue;
        private PnP.Framework.AuthenticationManager _authenticationManager;

        public SharepointManager(IOptions<SharepointSettings> sharepointSettings)
        {
            var settingsVal = sharepointSettings.Value;
            _azureAppId = settingsVal.AzureAppId;
            _sharepointUrl = settingsVal.SharepointUrl;
            SecureString securePassword = new SecureString();
            foreach(char c in settingsVal.Password)
            {
                securePassword.AppendChar(c);
            }
            _authenticationManager = new PnP.Framework.AuthenticationManager(_azureAppId, settingsVal.Username, securePassword);
        }

        public async Task<bool> AddItemToList(string listName, SharepointListItem item)
        {

            try
            {
                // Authentication.
                using (var context = _authenticationManager.GetContext(_sharepointUrl))
                {
                    Microsoft.SharePoint.Client.List eventList = context.Web.Lists.GetByTitle(listName);
                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();

                    ListItem listItemToAdd = eventList.AddItem(itemInfo);
                    foreach (var field in item)
                    {
                        listItemToAdd[field.Key] = field.Value;
                    }
                    listItemToAdd.Update();
                    await context.ExecuteQueryAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error occurred while adding an item.");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<List<SharepointListItem>> GetItemsFromList(string listName)
        {
            var itemsToReturn = new List<SharepointListItem>();
            using (var context = _authenticationManager.GetContext(_sharepointUrl))
            {
                var list = context.Web.Lists.GetByTitle(listName);
                context.Load(list);
                context.Load(list.Fields);
                await context.ExecuteQueryAsync();

                CamlQuery query = CamlQuery.CreateAllItemsQuery();

                var items = list.GetItems(query);
                context.Load(items);
                await context.ExecuteQueryAsync();

                foreach (var item in items)
                {
                    itemsToReturn.Add(new SharepointListItem(item.FieldValues));
                }
                return itemsToReturn;
            }
        }

        public bool CheckListExists(string listName)
        {
            using (var context = _authenticationManager.GetContext(_sharepointUrl))
            {
                Web orcaSite = context.Web;
                return orcaSite.ListExists(listName);
            }
        }

        /// <inheritdoc/>
        public void CreateList(string listName, string description, List<string> fieldsAsXml)
        {
            using (var context = _authenticationManager.GetContext(_sharepointUrl))
            {
                Web orcaSite = context.Web;

                ListCreationInformation listCreationInfo = new ListCreationInformation();
                listCreationInfo.Title = listName;
                listCreationInfo.TemplateType = (int)ListTemplateType.GenericList;
                listCreationInfo.Description = description;
                listCreationInfo.QuickLaunchOption = QuickLaunchOptions.On;
                // The new list is displayed on the Quick Launch of the site.
                List catalogList = orcaSite.Lists.Add(listCreationInfo);
                foreach (var fieldXml in fieldsAsXml)
                {
                    catalogList.Fields.AddFieldAsXml(fieldXml, true, AddFieldOptions.DefaultValue);
                }

                // Hide the default title field (column).
                // When created the list, do not set the title as 'required'.
                Field title = orcaSite.Lists.GetByTitle(listName).Fields.GetByTitle("Title");
                title.Hidden = true;
                title.Required = false;
                title.Update();

                var defaultListView = catalogList.DefaultView;
                context.Load(defaultListView);
                context.Load(defaultListView.ViewFields);
                context.ExecuteQuery();
                defaultListView.ViewFields.Remove("LinkTitle");
                defaultListView.Update();

                // add a navigation link to the newly created list
                orcaSite.AddNavigationNode(
                    listName,
                    new Uri($"{_sharepointUrl}/Lists/{listName}"),
                    string.Empty,
                    PnP.Framework.Enums.NavigationType.QuickLaunch);

                // Change permissions, need to be improved.
                // Microsoft.SharePoint.Client.List targetList = orcaSite.Lists.GetByTitle(listName);
                // context.Load(targetList, targetListInfo => targetListInfo.HasUniqueRoleAssignments);
                // context.ExecuteQuery();

                // if (!targetList.HasUniqueRoleAssignments) {
                //     Console.WriteLine("Target list is inheriting role assignments.");
                //     targetList.BreakRoleInheritance(true, false);
                // } else {
                //     Console.WriteLine("Target list has unique role assignments.");
                // }
                // targetList.Update();
                // orcaSite.Update();
                // Console.WriteLine("Privilege modified.");
                // await context.ExecuteQueryAsync();
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _authenticationManager?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

    }
    
    public class SharepointListItem : Dictionary<string, object>
    {

        public SharepointListItem() : base()
        {
        }

        /// <summary>
        /// Creates a SharepointListItem based on a dictionary, where the keys are the field names and values are the field values
        /// </summary>
        /// <param name="dictionary">A dictionary whose keys/values represent the item's field names/field values</param>
        public SharepointListItem(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }


    }

}
