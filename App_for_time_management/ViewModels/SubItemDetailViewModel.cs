using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using App_for_time_management.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    [QueryProperty(nameof(SubItemId), nameof(SubItemId))]
    class SubItemDetailViewModel : BaseViewModel
    {
        private string subItemId;
        private string text;
        private string description;
        private TimeSpan duration;
        private SubActivity subItem;
        private string activityTitle;
        public Command<SubActivityNote> NoteTapped { get; }

        public SubItemDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            DoneCommand = new Command(OnDone);
            LoadSubItemCommand = new Command(OnAppear);
            SubActivityNotes = new ObservableCollection<SubActivityNote>();
            AddSubActivityNoteCommand = new Command(OnAddActivityNote);
            NoteTapped = new Command<SubActivityNote>(OnNoteTapped);
        }

        public ObservableCollection<SubActivityNote> SubActivityNotes { get; }
        public Command AddSubActivityNoteCommand { get; }
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
        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        public string SubItemId
        {
            get => subItemId;

            set => SetProperty(ref subItemId, value);
        }

        public string ActivityTitle
        {
            get => activityTitle;
            set => SetProperty(ref activityTitle, value);
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        public async void LoadSubItemId(string subItemId)
        {
            try
            {
                var t = DataStore.GetSubItemAsync(subItemId);
                
                subItem = await t;
                SubItemId = subItem.ID;
                Text = subItem.Text;
                Description = subItem.Description;
                Duration = subItem.Duration;
                ActivityTitle = subItem.ParentActivity.Name;
                lock (SubActivityNotes)
                {
                    SubActivityNotes.Clear();
                    foreach (var note in subItem.SubActivityNotes)
                    {
                        SubActivityNotes.Add(note);
                    }
                }
                

            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to Load Item");
                Debug.WriteLine(e.Message);
                Debug.WriteLine(subItemId);
            }
        }
        public Command DeleteCommand { get; }
        private async void OnDelete()
        {
            await DataStore.DeleteSubItemAsync(SubItemId);
            await Shell.Current.GoToAsync("..");

        }

        public Command DoneCommand { get; }

        private async void OnDone()
        {
            subItem.IsDone = true;
            await DataStore.UpdateSubItemAsync(subItem);
            await Shell.Current.GoToAsync("..");
        }

        public Command LoadSubItemCommand { get; }
        private async void OnAppear()
        {
            IsBusy = true;
            LoadSubItemId(SubItemId);
            IsBusy = false;
        }
        private async void OnAddActivityNote()
        {
            string result = await Shell.Current.DisplayPromptAsync("Nowa notatka", "Treść");
            if (result == null)
            {
                return;
            }
            SubActivityNote subActivityNote = new SubActivityNote
            {
                ID = Guid.NewGuid().ToString(),
                Content = result,
                ParentID = SubItemId
            };
            await DataStore.AddSubActivityNoteAsync(subActivityNote);
            SubActivityNotes.Add(subActivityNote);

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

        private async void OnNoteTapped(SubActivityNote note)
        {
            if (note == null)
            {
                return;
            }
            string result = await Shell.Current.DisplayPromptAsync("Notatka", "Treść", cancel: "Anuluj", initialValue: note.Content);
            if (!string.IsNullOrWhiteSpace(result))
            {
                int index = SubActivityNotes.IndexOf(note);
                SubActivityNotes.Remove(note);
                note.Content = result;
                SubActivityNotes.Insert(index, note);
            }
            await DataStore.UpdateSubActivityNote(note);
        }
    }
}
