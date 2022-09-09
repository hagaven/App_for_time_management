using App_for_time_management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App_for_time_management.Services
{
    public interface IDataStore
    {
        Task<int> AddItemAsync(Activity item);
        Task<int> UpdateItemAsync(Activity item);
        Task<int> DeleteItemAsync(string id);
        Task<Activity> GetItemAsync(string id);
        Task<IEnumerable<Activity>> GetItemsAsync(bool forceRefresh = false);

        Task<int> AddSubItemAsync(SubActivity subItem);
        Task<int> UpdateSubItemAsync(SubActivity subItem);
        Task<int> DeleteSubItemAsync(string id);
        Task<SubActivity> GetSubItemAsync(string id);
        Task<IEnumerable<SubActivity>> GetSubItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<SubActivity>> GetSubItemsByParentIDAsync(string id, bool forceRefresh = false);

        Task<int> AddActivityNoteAsync(ActivityNote activityNote);
        Task<int> DeleteActivityNoteAsync(string id);
        Task<ActivityNote> GetActivityNoteAsync(string id);
        Task<int> UpdateActivityNote(ActivityNote activityNote);

        Task<int> AddSubActivityNoteAsync(SubActivityNote subActivityNote);
        Task<int> DeleteSubActivityNoteAsync(string id);
        Task<SubActivityNote> GetSubActivityNoteAsync(string id);
        Task<int> UpdateSubActivityNote(SubActivityNote subActivityNote);

    }
}
