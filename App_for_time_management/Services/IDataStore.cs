using App_for_time_management.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App_for_time_management.Services
{
    public interface IDataStore
    {
        Task<int> AddItemAsync(Item item);
        Task<int> UpdateItemAsync(Item item);
        Task<int> DeleteItemAsync(string id);
        Task<Item> GetItemAsync(string id);
        Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false);
        Task<int> AddSubItemAsync(SubItem subItem);
        Task<int> UpdateSubItemAsync(SubItem subItem);
        Task<int> DeleteSubItemAsync(string id);
        Task<SubItem> GetSubItemAsync(string id);
        Task<IEnumerable<SubItem>> GetSubItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<SubItem>> GetSubItemsByParentIDAsync(string id,bool forceRefresh = false);

    }
}
