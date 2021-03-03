﻿using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Orca.Entities;
using Orca.Services;
using Orca.Tools;
using OrcaTests.Services;
using OrcaTests.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrcaTests.Integration
{
    /// <summary>
    /// Tests in this class require the following environment variables to run:
    /// Pre: ORCA_INTEGRATION = TRUE
    /// AZURE_APP_ID 
    /// SHAREPOINT_URL
    /// SHAREPOINT_USERNAME
    /// SHAREPOINT_PASSWORD
    /// </summary>
    public class SharepointIntegrationTests : IDisposable
    {
        private string _listNameForTest;

        // Before run this integration test, please do SharePoint environment variable settings.
        // By default you can set as following:
        // AZURE_APP_ID="b269d983-e626-4d2d-bf17-606b0f2a93bb"
        // SHAREPOINT_URL="https://liveuclac.sharepoint.com/sites/ORCA"
        // SHAREPOINT_USERNAME and SHAREPOINT_PASSWORD is one tester who has permissions on that Sharepoint list.

        // Test functions(methods) in SharepointManager.cs (namespace Orca.Tools)
        // AddItemToList();
        // GetItemsFromList();
        // CheckListExists();
        // CreateList();

        public SharepointIntegrationTests()
        {
            // generate random list name to avoid collisions
            _listNameForTest = "SharepointIntegrationTest-" + Guid.NewGuid();
        }

        // Testing for this function need the env variable ORCA_INTEGRATION = TRUE.
        [IntegrationFact]
        public async void CreateListAndAddOrGetItemsTest()
        {
            var sharepointManager = new SharepointManager(Options.Create(SharepointSettingsFromEnv()));
            var listToCreate = _listNameForTest;
            var description = "Sharepoint integration test simple list.";
            string FIELD_XML_SCHEMA = $"<Field DisplayName='Default' Type='Text' Required='TRUE' />";

            bool listExistsBeforeCreating = sharepointManager.CheckListExists(listToCreate);
            Assert.False(listExistsBeforeCreating);

            sharepointManager.CreateList(listToCreate, description, new List<string> { FIELD_XML_SCHEMA });
            
            bool listExistsAfterCreating = sharepointManager.CheckListExists(listToCreate);
            Assert.True(listExistsAfterCreating);

            SharepointListItem testItem = new SharepointListItem();
            testItem["Default"] = "testItem";

            var listBeforeAdding = await sharepointManager.GetItemsFromList(listToCreate);
            int numOfItemsBeforeAdding = listBeforeAdding.Count;
            Assert.Equal(0, numOfItemsBeforeAdding);

            var addItem = await sharepointManager.AddItemToList(listToCreate, testItem);
            bool addItemSuccessful = addItem;
            // Add item to list should be successful.
            Assert.True(addItemSuccessful);
            if (addItemSuccessful)
            {
                var listAfterAdding = await sharepointManager.GetItemsFromList(listToCreate);
                int numOfItemsAfterAdding = listAfterAdding.Count;
                Assert.Equal(1, numOfItemsAfterAdding);
                
                SharepointListItem item = listAfterAdding.First();
                bool itemContainsExpectedKey = item.Keys.Contains("Default");
                bool itemContainsExpectedValue = item.Values.Contains("testItem");
                Assert.True(itemContainsExpectedKey && itemContainsExpectedValue);
                Assert.Equal("testItem", item["Default"]);
            }
        }

        [IntegrationFact]
        public async void AddItemToListButFailedTest()
        {
            var sharepointManager = new SharepointManager(Options.Create(SharepointSettingsFromEnv()));
            var listToCreate = _listNameForTest;

            bool listExists = sharepointManager.CheckListExists(listToCreate);
            Assert.False(listExists);

            SharepointListItem testItem = new SharepointListItem();
            testItem["Default"] = "testItem";

            var addItem = await sharepointManager.AddItemToList(listToCreate, testItem);
            bool addItemSuccessful = addItem;
            Assert.False(addItemSuccessful);
        }

        private static SharepointSettings SharepointSettingsFromEnv()
        {
            var settings = new SharepointSettings
            {
                AzureAppId = Environment.GetEnvironmentVariable("AZURE_APP_ID"),
                SharepointUrl = Environment.GetEnvironmentVariable("SHAREPOINT_URL"),
                Username = Environment.GetEnvironmentVariable("SHAREPOINT_USERNAME"),
                Password = Environment.GetEnvironmentVariable("SHAREPOINT_PASSWORD"),
                CourseCatalogListName = "defaultName"
                // No need for course catalog for this class' testing.
                
            };
            return settings;

        }

        /// <summary>
        /// Delete sharepoint list after each test if it was created.
        /// </summary>
        public void Dispose()
        {
            var spSettings = SharepointSettingsFromEnv();
            SecureString securePassword = new SecureString();
            foreach (char c in spSettings.Password)
            {
                securePassword.AppendChar(c);
            }
            using (var _authenticationManager = new PnP.Framework.AuthenticationManager(spSettings.AzureAppId, spSettings.Username, securePassword))
            {
                using (var context = _authenticationManager.GetContext(spSettings.SharepointUrl))
                {
                    var orcaSite = context.Web;
                    if (orcaSite.ListExists(_listNameForTest))
                    {
                        var list = orcaSite.Lists.GetByTitle(_listNameForTest);
                        list.DeleteObject();
                        orcaSite.DeleteNavigationNode(_listNameForTest, string.Empty, PnP.Framework.Enums.NavigationType.QuickLaunch);
                        context.ExecuteQuery();
                    }
                }
            }
        }
    }
}
