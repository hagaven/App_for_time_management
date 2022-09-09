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
        private Activity item;
        private string itemId;
        private string name;
        private string description;
        private DateTime deadlineDate;
        private TimeSpan deadlineTime;
        private string eisenhower;
        private string timeSensitive;
        private bool isCyclic;
        private string cyclePeriod;
        private TimeSpan duration;
        private bool isDone;
        private bool cycleVisibility;


        public ObservableCollection<SubActivity> SubActivities { get; }
        public ObservableCollection<ActivityNote> ActivityNotes { get; set; }
        public Command AddActivityNoteCommand { get; private set; }
        public ICommand LoadSubItemsCommand { get; }
        public Command<SubActivity> SubItemTapped { get; }

        public Command<ActivityNote> NoteTapped { get; }

        private SubActivity _selectedSubItem;

        public double ListHeight { get; set; } = 50;


        public ItemDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            DoneCommand = new Command(OnDone);
            LoadSubItemsCommand = new Command(async () => await ExecuteLoadSubItemsCommand());
            SubItemTapped = new Command<SubActivity>(OnSubItemSelected);
            SubActivities = new ObservableCollection<SubActivity>();
            ActivityNotes = new ObservableCollection<ActivityNote>();
            AddActivityNoteCommand = new Command(OnAddActivityNote);
            AddSubActivityCommand = new Command(OnAddSubActivity);
            NoteTapped = new Command<ActivityNote>(OnNoteTapped);
        }

        private async void OnNoteTapped(ActivityNote note)
        {
            if (note == null)
            {
                return;
            }

            string result = await Shell.Current.DisplayPromptAsync("Notatka", "Treść", cancel: "Anuluj", initialValue: note.Content);
            if (!string.IsNullOrWhiteSpace(result))
            {
                int index = ActivityNotes.IndexOf(note);
                if (ActivityNotes.Remove(note))
                {
                    Debug.WriteLine("Succesfully removed note");
                }
                else
                {
                    Debug.WriteLine("Error occured during removing note: couldn't remove note or note wasn't present in collection");
                }
                note.Content = result;
                ActivityNotes.Insert(index, note);
            }
            int v = await DataStore.UpdateActivityNote(note);
            Debug.WriteLine("Successfully updated " + v + " note");
        }

        public string Id { get; set; }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
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
            get => itemId;
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
        public bool IsCyclic
        {
            get => isCyclic;
            set
            {
                _ = SetProperty(ref isCyclic, value);
                item.IsCyclic = value;
                _ = DataStore.UpdateItemAsync(item);
            }
        }
        public string CyclePeriod
        {
            get => cyclePeriod;
            set
            {
                _ = SetProperty(ref cyclePeriod, value);
                item.CyclePeriod = value;
                _ = DataStore.UpdateItemAsync(item);
            }
        }
        public SubActivity SelectedSubItem
        {
            get => _selectedSubItem;
            set => SetProperty(ref _selectedSubItem, value);
        }

        public bool CycleVisibility
        {
            get => cycleVisibility;
            set => SetProperty(ref cycleVisibility, value);
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
                IsBusy = true;
                Task<Activity> t = DataStore.GetItemAsync(itemId);
                item = await t;
                Id = item.ID;
                Name = item.Name;
                Description = item.Description;
                DeadlineDate = item.DeadlineDate;
                DeadlineTime = item.DeadlineTime;
                Eisenhower = item.Eisenhower;
                TimeSensitive = IsTimeSensitive(item.TimeSensitive);
                CycleVisibility = item.TimeSensitive;
                IsCyclic = item.IsCyclic;
                CyclePeriod = item.CyclePeriod;
                Duration = item.Duration;
                IsDone = item.IsDone;

                lock (SubActivities)
                {
                    SubActivities.Clear();
                    foreach (SubActivity subItem in item.SubActivity)
                    {
                        SubActivities.Add(subItem);
                    }
                }
                lock (ActivityNotes)
                {
                    ActivityNotes.Clear();
                    foreach (ActivityNote note in item.ActivityNotes)
                    {
                        ActivityNotes.Add(note);
                    }
                }

                IsBusy = false;

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
            int v = await DataStore.DeleteItemAsync(Id);
            Debug.WriteLine("Succesfully deleted " + v + " items");
            await Shell.Current.GoToAsync("..");
        }

        public Command DoneCommand { get; }

        private async void OnDone()
        {
            item.IsDone = true;
            _ = await DataStore.UpdateItemAsync(item);
            await Shell.Current.GoToAsync("..");
        }
        public Command AddSubActivityCommand { get; }

        public string IsTimeSensitive(bool timeSensitivity)
        {
            return timeSensitivity ? "Aktywność wrażliwa na czas wykonania" : "Aktywność nie jest wrażliwa na czas wykonania";
        }

        private async void OnSubItemSelected(SubActivity subItem)
        {
            if (subItem == null)
            {
                return;
            }
            await Shell.Current.GoToAsync($"{nameof(SubItemDetailPage)}?{nameof(SubItemDetailViewModel.SubItemId)}={subItem.ID}");
        }
        private async Task ExecuteLoadSubItemsCommand()
        {
            IsBusy = true;

            try
            {
                System.Collections.Generic.IEnumerable<SubActivity> subItems = await App.Database.GetSubItemsByParentIDAsync(Id);
                SubActivities.Clear();

                foreach (SubActivity subItem in subItems)
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
        private async void OnAddActivityNote()
        {
            string result = await Shell.Current.DisplayPromptAsync("Nowa notatka", "Treść");
            if (result == null)
            {
                return;
            }
            ActivityNote activityNote = new ActivityNote
            {
                ID = Guid.NewGuid().ToString(),
                Content = result,
                ParentID = Id
            };
            int v = await DataStore.AddActivityNoteAsync(activityNote);
            Debug.WriteLine("Succesfully added " + v + " note(s)");
            ActivityNotes.Add(activityNote);

        }
        private async void OnAddSubActivity()
        {
            await Shell.Current.GoToAsync($"{nameof(NewSubItemPage)}?{nameof(NewSubItemViewModel.ParentID)}={Id}");
        }

        private Command backCommand;

        public ICommand BackCommand
        {
            get
            {
                if (backCommand == null)
                {
                    backCommand = new Command(Back);
                }

                return backCommand;
            }
        }

        private async void Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
