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
        Task<int> DeleteItemAsync(int id);
        Task<Item> GetItemAsync(int id);
        Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false);

    }
}
