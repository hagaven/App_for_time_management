using App_for_time_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace App_for_time_management.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;
        private static SQLiteAsyncConnection database;

        public MockDataStore()
        {
            
            items = new List<Item>();

        }
        static async Task Init()
        {
            if (database != null)
            {
                return;
            }

            string dbPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "base.db3");
            database = new SQLiteAsyncConnection(dbPath);
            await database.CreateTableAsync<Item>();
            await database.CreateTableAsync<SubItem>();

        }

        public async Task<int> AddItemAsync(Item item)
        {
            items.Add(item);
            await Init();
            return await database.InsertAsync(item); ;
        }

        public async Task<int> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.ID == item.ID).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            await Init();
            return await database.UpdateAsync(item);
        }

        public async Task<int> DeleteItemAsync(int id)
        {
            var oldItem = items.Where((Item arg) => arg.ID == id).FirstOrDefault();
            items.Remove(oldItem);
            await Init();
            return await database.DeleteAsync<Item>(id);
        }

        public async Task<Item> GetItemAsync(int id)
        {
            await Init();
            //await Task.FromResult(items.FirstOrDefault(s => s.ID == id));
            
            return await database.Table<Item>().FirstOrDefaultAsync(s => s.ID == id);
        }



        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            //await Task.FromResult(items);
            //await database.Table<Item>().ToListAsync();
            var itemList = await database.Table<Item>().ToListAsync();
            return itemList;
        }
    }
}