using App_for_time_management.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
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
        private readonly string id;
        private string parentID;



        public NewSubItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            
            this.id = Guid.NewGuid().ToString();

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

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }


        private async void OnCancel()
        {
            
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            SubItem newSubItem = new SubItem()
            {
                ID=this.id,
                Text = Text,
                Description = Description,
                Duration = duration,
                ParentID = parentID



            };
            await App.Database.AddSubItemAsync(newSubItem);
            await Shell.Current.GoToAsync("..");
        }
    }
}
