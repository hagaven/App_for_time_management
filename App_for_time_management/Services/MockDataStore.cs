using App_for_time_management.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using SQLiteNetExtensionsAsync.Extensions;

namespace App_for_time_management.Services
{
    public class MockDataStore : IDataStore
    {
        private readonly List<Item> items;
        private readonly List<SubItem> subItems;
        private static SQLiteAsyncConnection database;

        public MockDataStore()
        {
            
            items = new List<Item>();
            subItems = new List<SubItem>();

        }
        static async Task Init()
        {
            if (database != null)
            {
                return;
            }
            string dbPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "base.db3");
            database = new SQLiteAsyncConnection(dbPath);
            //await database.DropTableAsync<SubItem>();
            //await database.DropTableAsync<Item>();
            await database.CreateTableAsync<Item>();
            await database.CreateTableAsync<SubItem>();
            await database.CreateTableAsync<ActivityNote>();
            await database.CreateTableAsync<SubActivityNote>();


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

        public async Task<int> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.ID == id).FirstOrDefault();
            items.Remove(oldItem);
            await Init();
            return await database.DeleteAsync<Item>(id);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            await Init();
            return await database.GetWithChildrenAsync<Item>(id);
        }



        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            var itemList = await database.GetAllWithChildrenAsync<Item>();
            return itemList;
        }

        public async Task<int> AddSubItemAsync(SubItem subItem)
        {
            subItems.Add(subItem);
            await Init();
            return await database.InsertAsync(subItem);
        }

        public async Task<int> UpdateSubItemAsync(SubItem subItem)
        {
            var oldSubItem = subItems.Where((SubItem arg) => arg.ID == subItem.ID).FirstOrDefault();
            subItems.Remove(oldSubItem);
            subItems.Add(subItem);
            await Init();
            return await database.UpdateAsync(subItem);
        }

        public async Task<int> DeleteSubItemAsync(string id)
        {
            var oldSubItem = subItems.Where((SubItem arg) => arg.ID == id).FirstOrDefault();
            subItems.Remove(oldSubItem);
            await Init();
            return await database.DeleteAsync<SubItem>(id);
        }

        public async Task<SubItem> GetSubItemAsync(string id)
        {
            await Init();
            
            return await database.GetWithChildrenAsync<SubItem>(id);
        }



        public async Task<IEnumerable<SubItem>> GetSubItemsAsync(bool forceRefresh = false)
        {
            await Init();
            var subItemList = await database.Table<SubItem>().ToListAsync();
            return subItemList;
        }
        public async Task<IEnumerable<SubItem>> GetSubItemsByParentIDAsync(string id,bool forceRefresh = false)
        {
            await Init();
            Item it = await GetItemAsync(id);
            return it.SubActivity;
        }

        public async Task<int> AddActivityNoteAsync(ActivityNote activityNote)
        {
            await Init();
            return await database.InsertAsync(activityNote);

        }
        public async Task<int> DeleteActivityNoteAsync(string id)
        {
            await Init();
            return await database.DeleteAsync<ActivityNote>(id);
        }
        public async Task<ActivityNote> GetActivityNoteAsync(string id)
        {
            await Init();
            return await database.Table<ActivityNote>().FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<int> AddSubActivityNoteAsync(SubActivityNote subActivityNote)
        {
            await Init();
            return await database.InsertAsync(subActivityNote);
        }
        public async Task<int> DeleteSubActivityNoteAsync(string id)
        {
            await Init();
            return await database.DeleteAsync<ActivityNote>(id);
        }
        public async Task<SubActivityNote> GetSubActivityNoteAsync(string id)
        {
            await Init();
            return await database.Table<SubActivityNote>().FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<int> UpdateActivityNote(ActivityNote activityNote)
        {
            await Init();
            return await database.UpdateAsync(activityNote);
        }

        public async Task<int> UpdateSubActivityNote(SubActivityNote subActivityNote)
        {
            await Init();
            return await database.UpdateAsync(subActivityNote);
        }
    }
}