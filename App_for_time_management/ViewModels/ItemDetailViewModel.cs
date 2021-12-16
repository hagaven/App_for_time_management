using App_for_time_management.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private Item item;
        private int itemId;
        private string text;
        private string description;
        private DateTime deadlineDate;
        private TimeSpan deadlineTime;
        private string eisenhower;
        private string timeSensitive;
        private TimeSpan duration;



        public ItemDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            DoneCommand = new Command(OnDone);
        }

        public int Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string Eisenhower
        {
            get => eisenhower;
            set => SetProperty(ref eisenhower, value);
        }
        public string TimeSensitive
        {
            get => timeSensitive;
            set => SetProperty(ref timeSensitive, value);
        }
        public DateTime DeadlineDate
        {
            get => deadlineDate.Date;
            set => SetProperty(ref deadlineDate, value);
        }

        public TimeSpan DeadlineTime
        {
            get => deadlineTime;
            set => SetProperty(ref deadlineTime, value);
        }
        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        public int ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                var t = DataStore.GetItemAsync(itemId);
                
                item = await t;
                Id = item.ID;
                Text = item.Text;
                Description = item.Description;
                DeadlineDate = item.DeadlineDate;
                DeadlineTime = item.DeadlineTime;
                Eisenhower = item.Eisenhower;
                TimeSensitive = IsTimeSensitive(item.TimeSensitive);
                Duration = item.Duration;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to Load Item");
                Debug.WriteLine(e.Message);
                Debug.WriteLine(itemId);
            }
        }
        public Command DeleteCommand { get; }
        private async void OnDelete()
        {
            await DataStore.DeleteItemAsync(Id);
            await Shell.Current.GoToAsync("..");
            
        }

        public Command DoneCommand { get; }

        private async void OnDone()
        {
            item.IsDone = true;
            await DataStore.UpdateItemAsync(item);
            await Shell.Current.GoToAsync("..");
        }

        public string IsTimeSensitive(bool timeSensitivity)
        {
            return timeSensitivity ? "Aktywnosść wrażliwa na czas wykonania" : "Aktywnosść nie jest wrażliwa na czas wykonania";
        }

       
    }
}
