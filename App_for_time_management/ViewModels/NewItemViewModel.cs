using App_for_time_management.Models;
using System;
using System.Collections.Generic;
using System.Text;
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




        public NewItemViewModel()
        {
            
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
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
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }



        private async void OnSave()
        {
            TimeSpan duration = new TimeSpan(DurationHours, DurationMinutes,0);
            Item newItem = new Item()
            {
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

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

       
    }
}
