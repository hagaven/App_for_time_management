using App_for_time_management.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    [QueryProperty(nameof(ParentID), nameof(ParentID))]
    public class NewSubItemViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private int durationHours;
        private int durationMinutes;
        private string durctrl1;
        private string durctrl2;
        private string id;
        private string parentID;
        public Command<SubActivityNote> NoteTapped { get; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddSubActivityNoteCommand { get; }
        public ObservableCollection<SubActivityNote> SubActivityNotes { get; set; }

        public NewSubItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddSubActivityNoteCommand = new Command(OnAddActivityNote);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            
            id = Guid.NewGuid().ToString();
            SubActivityNotes = new ObservableCollection<SubActivityNote>();
            NoteTapped = new Command<SubActivityNote>(OnNoteTapped);
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

        public string ParentID
        {
            get => parentID;
            set => SetProperty(ref parentID, value);
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

        private async void OnCancel()
        {
            
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            SubActivity newSubItem = new SubActivity()
            {
                ID = id,
                Text = Text,
                Description = Description,
                Duration = duration,
                ParentID = parentID
            };
            await App.Database.AddSubItemAsync(newSubItem);
            await Shell.Current.GoToAsync("..");
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
                ParentID = id
            };
            await DataStore.AddSubActivityNoteAsync(subActivityNote);
            SubActivityNotes.Add(subActivityNote);

        }
    }
}
