using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private DateTime deadlineDate;
        private TimeSpan deadlineTime;
        private string eisenhower;
        private bool timesensitive;
        private int durationHours;
        private int durationMinutes;
        private string durctrl1;
        private string durctrl2;
        private bool alreadyPresent;
        public ObservableCollection<SubItem> SubActivities { get; }
        public double ListHeight { get; set; } = 100;
        public Command<SubItem> SubItemTapped { get; }
        private SubItem _selectedSubItem;

        private readonly string id;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddSubActivityCommand { get; }

        public ICommand LoadSubItemsCommand { get; }


        public NewItemViewModel()
        {
            
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddSubActivityCommand = new Command(OnAddSubActivity);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            id = Guid.NewGuid().ToString();
            SubActivities = new ObservableCollection<SubItem>();
            SubItemTapped = new Command<SubItem>(OnSubItemSelected);
            LoadSubItemsCommand = new Command(async () => await ExecuteLoadSubItemsCommand());
            alreadyPresent = false;
        }
        public async void LoadItem()
        {
            try
            {
                var t = DataStore.GetItemAsync(id);

                Item item = await t;
                
                Text = item.Text;
                Description = item.Description;
                DeadlineDate = item.DeadlineDate;
                DeadlineTime = item.DeadlineTime;
                Eisenhower = item.Eisenhower;
                TimeSensitive = item.TimeSensitive;
                DurCntrl1 = item.Duration.Hours.ToString();
                DurCntrl2 = item.Duration.Minutes.ToString();
                alreadyPresent = true;
            }
            catch (Exception)
            {
                
            }
        }

        public async void OnAppearing()
        {
            IsBusy = true;
            SelectedSubItem = null;
            LoadItem();
            await ExecuteLoadSubItemsCommand();
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
        private async void OnAddSubActivity()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes, 0);

            Item newItem = new Item()
            {
                ID = id,
                Text = Text,
                Description = Description,
                DeadlineDate = DeadlineDate.Add(DeadlineTime),
                DeadlineTime = DeadlineTime,
                Eisenhower = Eisenhower,
                TimeSensitive = TimeSensitive,
                IsDone = false,
                AdditionDate = DateTime.Now,
                Duration = duration

            };

            if (!alreadyPresent)
            {
                await DataStore.AddItemAsync(newItem);
            }
            else
            {
                await DataStore.UpdateItemAsync(newItem);
            }
            await Shell.Current.GoToAsync($"{nameof(NewSubItemPage)}?{nameof(NewSubItemViewModel.ParentID)} = {id}");
            
            
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text)
                && !String.IsNullOrWhiteSpace(description);
        }

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
        public DateTime Today{
            get => DateTime.Now;
        }
        public string Eisenhower
        {
            get => eisenhower;
            set => SetProperty(ref eisenhower, value);
        }

        public DateTime DeadlineDate
        {
            get => deadlineDate;
            set => SetProperty(ref deadlineDate, value);
        }
        public TimeSpan DeadlineTime
        {
            get => deadlineTime;
            set => SetProperty(ref deadlineTime, value);
        }

        public bool TimeSensitive
        {
            get => timesensitive;
            set => SetProperty(ref timesensitive, value);
        }

        public int DurationHours
        {
            get => durationHours;
            set => SetProperty(ref durationHours, value);
        }

        public int DurationMinutes
        {
            get => durationMinutes;
            set => SetProperty(ref durationMinutes, value);
        }

        public string DurCntrl1
        {
            get => durctrl1;
            set => SetProperty(ref durctrl1, value);
        }

        public string DurCntrl2
        {
            get => durctrl2;
            set => SetProperty(ref durctrl2, value);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }



        private async void OnSave()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes,0);
            List<SubItem> subs = new List<SubItem>();
            foreach(var s in SubActivities)
            {
                subs.Add(s);
            };
            Item newItem = new Item()
            {
                ID = id,
                Text = Text,
                Description = Description,
                DeadlineDate = DeadlineDate.Add(DeadlineTime),
                DeadlineTime = DeadlineTime,
                Eisenhower = Eisenhower,
                TimeSensitive = TimeSensitive,
                IsDone = false,
                AdditionDate = DateTime.Now,
                Duration = duration,
                SubActivity = subs

            };
            if (!alreadyPresent)
            {
                await DataStore.AddItemAsync(newItem);
            }
            else
            {
                await DataStore.UpdateItemAsync(newItem);
            }
            

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
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
                var subItems = await App.Database.GetSubItemsByParentIDAsync(id, true);
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
