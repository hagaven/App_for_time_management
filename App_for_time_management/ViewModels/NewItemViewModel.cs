using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    public class NewItemViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string text;
        private string description;
        private DateTime deadlineDate;
        private TimeSpan deadlineTime;
        private string eisenhower;
        private bool timesensitive;
        private bool isCyclic;
        private string cyclePeriod;
        private int durationHours;
        private int durationMinutes;
        private string durctrl1;
        private string durctrl2;
        private bool alreadyPresent;
        private string id;
        public ObservableCollection<SubItem> SubActivities { get; set; }
        public ObservableCollection<ActivityNote> ActivityNotes { get; set; }
        public double ListHeight { get; set; } = 100;
        public Command<SubItem> SubItemTapped { get; }
        private SubItem _selectedSubItem;
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddSubActivityCommand { get; }
        public Command AddActivityNoteCommand { get; }
        public Command<ActivityNote> NoteTapped { get; }
        public ICommand LoadSubItemsCommand { get; }
        public NewItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddSubActivityCommand = new Command(OnAddSubActivity);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            id = Guid.NewGuid().ToString();
            SubActivities = new ObservableCollection<SubItem>();
            ActivityNotes = new ObservableCollection<ActivityNote>();
            SubItemTapped = new Command<SubItem>(OnSubItemSelected);
            LoadSubItemsCommand = new Command(async () => await ExecuteLoadSubItemsCommand());
            AddActivityNoteCommand = new Command(OnAddActivityNote);
            alreadyPresent = false;
            DeadlineDate = DateTime.Now;
            NoteTapped = new Command<ActivityNote>(OnNoteTapped); 
        }
        public async void LoadItem()
        {
            try
            {
                var t = DataStore.GetItemAsync(id);

                Item item = await t;
                
                Text = item.Name;
                Description = item.Description;
                DeadlineDate = item.DeadlineDate;
                DeadlineTime = item.DeadlineTime;
                Eisenhower = item.Eisenhower;
                TimeSensitive = item.TimeSensitive;
                DurCntrl1 = item.Duration.Hours.ToString();
                DurCntrl2 = item.Duration.Minutes.ToString();
                alreadyPresent = true;
                lock (SubActivities)
                {
                    SubActivities.Clear();
                    foreach (var s in item.SubActivity)
                    {
                        SubActivities.Add(s);
                    }
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSubItem = null;
            LoadItem();
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
                Name = Text,
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
            await Shell.Current.GoToAsync($"{nameof(NewSubItemPage)}?{nameof(NewSubItemViewModel.ParentID)}={id}");
        }
        private async void OnAddActivityNote()
        {
            string result = await Shell.Current.DisplayPromptAsync("Nowa notatka", "Treść",accept: "OK", cancel:"Anuluj");
            if(result == null)
            {
                return;
            }
            ActivityNote activityNote = new ActivityNote
            {
                ID = Guid.NewGuid().ToString(),
                Content = result,
                ParentID = id
            };
            await DataStore.AddActivityNoteAsync(activityNote);
            ActivityNotes.Add(activityNote);

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
                ActivityNotes.Remove(note);
                note.Content = result;
                ActivityNotes.Insert(index, note);
            }
            await DataStore.UpdateActivityNote(note);
        }
        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }
        private async void OnSave()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes, 0);

            Item newItem = new Item()
            {
                ID = id,
                Name = Text,
                Description = Description,
                DeadlineDate = DeadlineDate.Add(DeadlineTime),
                DeadlineTime = DeadlineTime,
                Eisenhower = Eisenhower,
                TimeSensitive = TimeSensitive,
                IsDone = false,
                AdditionDate = DateTime.Now,
                Duration = duration,
                IsCyclic = IsCyclic,
                CyclePeriod = CyclePeriod

            };
            if (!alreadyPresent)
            {
                await DataStore.AddItemAsync(newItem);
            }
            else
            {
                await DataStore.UpdateItemAsync(newItem);
            }
            await Shell.Current.GoToAsync("..");
        }
        private async void OnSubItemSelected(SubItem subItem)
        {
            if (subItem == null)
                return;


            await Shell.Current.GoToAsync($"{nameof(SubItemDetailPage)}?{nameof(SubItemDetailViewModel.SubItemId)}={subItem.ID}");
        }
        private async Task ExecuteLoadSubItemsCommand()
        {
            IsBusy = true;

            try
            {   
                var subItems = await App.Database.GetSubItemsByParentIDAsync(id, true);
                lock (SubActivities)
                {
                    SubActivities.Clear();
                    foreach (var subItem in subItems)
                    {
                        SubActivities.Add(subItem);
                    }
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
        public DateTime Today => DateTime.Now;
        public string Eisenhower
        {
            get => eisenhower;
            set => SetProperty(ref eisenhower, value);
        }
        public bool IsCyclic
        {
            get => isCyclic;
            set => SetProperty(ref isCyclic, value);
        }
        public string CyclePeriod
        {
            get => cyclePeriod;
            set => SetProperty(ref cyclePeriod, value);
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
    }
}
