using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Tools;
namespace OrcaTests.Tools
{
    /// <summary>
    /// Dictionary-based implementation of the ISharepointManager used to help test classes which rely on Sharepoint
    /// </summary>
    public class MockSharepointManager : ISharepointManager
    {
        public readonly Dictionary<string, List<SharepointListItem>> mockEventList;
        public MockSharepointManager()
        {
            mockEventList = new Dictionary<string, List<SharepointListItem>>();
            SetInitialItemsInList();

        }
        public void SetInitialItemsInList()
        {
            mockEventList.Add("Announcements", new List<SharepointListItem>());
            mockEventList["Announcements"].Add(new SharepointListItem()
            {
                ["Course"] = "Comp102",
                ["StudentName"] = "Terry White"
            });
            mockEventList["Announcements"].Add(new SharepointListItem()
            {
                ["Course"] = "Comp104",
                ["StudentName"] = "Bill Burner"
            });

            mockEventList.Add("Information", new List<SharepointListItem>());
            mockEventList["Information"].Add(new SharepointListItem());
            mockEventList["Information"].Add(new SharepointListItem()
            {
                ["Course"] = "Comp103",
                ["StudentName"] = "Terry White"
            });
        }
        public void PrintItems()
        {
            foreach (var field in mockEventList)
            {
                foreach (var fields in mockEventList[field.Key])
                {
                    Console.WriteLine(fields);
                }
            }
        }
        public async Task<bool> AddItemToList(string listName, SharepointListItem item)
        {
            mockEventList[listName].Add(item);
            return true;
        }

        public async Task<List<SharepointListItem>> GetItemsFromList(string listName)
        {

            var itemsToReturn = new List<SharepointListItem>();
            for (var i = 0; i < mockEventList[listName].Count; ++i)
            {
                Dictionary<string, object> mockDictionary = new Dictionary<string, object>();
                foreach (var field in mockEventList[listName][i])
                {
                    mockDictionary.Add(field.Key, field.Value);
                }
                itemsToReturn.Add(new SharepointListItem(mockDictionary));

            }
            return itemsToReturn;
        }

        public bool CheckListExists(string listName)
        {
            return mockEventList.ContainsKey(listName);
        }

        /// <summary>
        /// Creates a new Sharepoint List. The description and fieldsAsXml params can be anything as they aren't used in this mock
        /// </summary>
        /// <param name="listName">Name of the list to create</param>
        /// <param name="description">Not used, can be anything</param>
        /// <param name="fieldsAsXml">Not used, can be anything</param>
        public void CreateList(string listName, string description, List<string> fieldsAsXml)
        {
            mockEventList.Add(listName, new List<SharepointListItem>());
        }

        public void Dispose()
        {
        }
    }
}
