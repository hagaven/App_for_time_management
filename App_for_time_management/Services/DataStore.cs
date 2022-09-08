using App_for_time_management.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using SQLiteNetExtensionsAsync.Extensions;

namespace App_for_time_management.Services
{
    public class DataStore : IDataStore
    {
        private readonly List<Activity> items;
        private readonly List<SubActivity> subItems;
        private static SQLiteAsyncConnection database;

        public DataStore()
        {
            
            items = new List<Activity>();
            subItems = new List<SubActivity>();

        }
        static async Task Init()
        {
            if (database != null)
            {
                return;
            }
            string dbPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "base.db3");
            database = new SQLiteAsyncConnection(dbPath);
            await database.CreateTableAsync<Activity>();
            await database.CreateTableAsync<SubActivity>();
            await database.CreateTableAsync<ActivityNote>();
            await database.CreateTableAsync<SubActivityNote>();


        }

        public async Task<int> AddItemAsync(Activity item)
        {
            items.Add(item);
            await Init();
            return await database.InsertAsync(item); ;
        }

        public async Task<int> UpdateItemAsync(Activity item)
        {
            var oldItem = items.Where((Activity arg) => arg.ID == item.ID).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            await Init();
            return await database.UpdateAsync(item);
        }

        public async Task<int> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Activity arg) => arg.ID == id).FirstOrDefault();
            items.Remove(oldItem);
            await Init();
            return await database.DeleteAsync<Activity>(id);
        }

        public async Task<Activity> GetItemAsync(string id)
        {
            await Init();
            return await database.GetWithChildrenAsync<Activity>(id);
        }



        public async Task<IEnumerable<Activity>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            var itemList = await database.GetAllWithChildrenAsync<Activity>();
            return itemList;
        }

        public async Task<int> AddSubItemAsync(SubActivity subItem)
        {
            subItems.Add(subItem);
            await Init();
            return await database.InsertAsync(subItem);
        }

        public async Task<int> UpdateSubItemAsync(SubActivity subItem)
        {
            var oldSubItem = subItems.Where((SubActivity arg) => arg.ID == subItem.ID).FirstOrDefault();
            subItems.Remove(oldSubItem);
            subItems.Add(subItem);
            await Init();
            return await database.UpdateAsync(subItem);
        }

        public async Task<int> DeleteSubItemAsync(string id)
        {
            var oldSubItem = subItems.Where((SubActivity arg) => arg.ID == id).FirstOrDefault();
            subItems.Remove(oldSubItem);
            await Init();
            return await database.DeleteAsync<SubActivity>(id);
        }

        public async Task<SubActivity> GetSubItemAsync(string id)
        {
            await Init();
            
            return await database.GetWithChildrenAsync<SubActivity>(id);
        }



        public async Task<IEnumerable<SubActivity>> GetSubItemsAsync(bool forceRefresh = false)
        {
            await Init();
            var subItemList = await database.Table<SubActivity>().ToListAsync();
            return subItemList;
        }
        public async Task<IEnumerable<SubActivity>> GetSubItemsByParentIDAsync(string id,bool forceRefresh = false)
        {
            await Init();
            Activity it = await GetItemAsync(id);
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