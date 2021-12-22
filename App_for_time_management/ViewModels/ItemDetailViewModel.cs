using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private Item item;
        private string itemId;
        private string text;
        private string description;
        private DateTime deadlineDate;
        private TimeSpan deadlineTime;
        private string eisenhower;
        private string timeSensitive;
        private TimeSpan duration;
        private bool isDone;


        public ObservableCollection<SubItem> SubActivities { get; }
        public ICommand LoadSubItemsCommand { get; }
        public Command<SubItem> SubItemTapped { get; }
        
        private SubItem _selectedSubItem;

        public double ListHeight { get; set; } = 50;
        

        public ItemDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            DoneCommand = new Command(OnDone);
            LoadSubItemsCommand = new Command(async () => await ExecuteLoadSubItemsCommand());
            SubItemTapped = new Command<SubItem>(OnSubItemSelected);
            SubActivities = new ObservableCollection<SubItem>();
            
        }

        public string Id { get; set; }

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

        public string ItemId
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

        public bool IsDone
        {
            get => isDone;
            set => SetProperty(ref isDone, value);
        }

        public SubItem SelectedSubItem
        {
            get => _selectedSubItem;
            set
            {
                SetProperty(ref _selectedSubItem, value);
                OnSubItemSelected(value);
            }
        }
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSubItem = null;
           
        }

        public async void LoadItemId(string itemId)
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
                IsDone = item.IsDone;
                
                
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
        public Command AddSubActivityCommand { get; }

        public string IsTimeSensitive(bool timeSensitivity)
        {
            return timeSensitivity ? "Aktywność wrażliwa na czas wykonania" : "Aktywność nie jest wrażliwa na czas wykonania";
        }

        private async void OnSubItemSelected(SubItem subItem)
        {
            if (subItem == null)
                return;


            await Shell.Current.GoToAsync($"{nameof(SubItemDetailPage)}?{nameof(SubItemDetailViewModel.Id)}={subItem.ID}");
        }
        private async Task ExecuteLoadSubItemsCommand()
        {
            IsBusy = true;

            try
            {
                SubActivities.Clear();
                var subItems = await App.Database.GetSubItemsByParentIDAsync(Id,true);
                foreach (var subItem in subItems)
                {
                    SubActivities.Add(subItem);
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
