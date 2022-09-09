using App_for_time_management.Views;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private TimeSpan startTime;
        private TimeSpan endTime;
        private int breakTime;
        private string breakCtrl;
        public TimeSpan StartTime
        {
            get => startTime;
            set => SetProperty(ref startTime, value);
        }
        public TimeSpan EndTime
        {
            get => endTime;
            set => SetProperty(ref endTime, value);
        }
        public int BreakTime
        {
            get => breakTime;
            set => SetProperty(ref breakTime, value);
        }

        public string BreakCtrl
        {
            get => breakCtrl;
            set => SetProperty(ref breakCtrl, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public SettingsViewModel()
        {
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            DateTime now = DateTime.Now;
            Debug.WriteLine(Preferences.Get("start", new DateTime(now.Year, now.Month, now.Day, 8, 0, 0)));
            Debug.WriteLine(Preferences.Get("end", new DateTime(now.Year, now.Month, now.Day, 22, 0, 0)));
            DateTime start = Preferences.Get("start", new DateTime(now.Year, now.Month, now.Day, 8, 0, 0));
            DateTime end = Preferences.Get("end", new DateTime(now.Year, now.Month, now.Day, 22, 0, 0));
            TimeSpan startDay = new TimeSpan(start.Hour, start.Minute, start.Second);
            TimeSpan endDay = new TimeSpan(end.Hour, end.Minute, end.Second);
            StartTime = startDay;
            EndTime = endDay;
            BreakTime = Preferences.Get("break", 10);
            BreakCtrl = BreakTime.ToString();
            Debug.WriteLine(Preferences.ContainsKey("start"));
            Debug.WriteLine(Preferences.ContainsKey("end"));
            Debug.WriteLine(Preferences.ContainsKey("break"));
        }

        private async void OnCancel()
        {
            Debug.WriteLine("Cancelation");
            await Shell.Current.GoToAsync("///" + nameof(AboutPage));
        }
        private async void OnSave()
        {
            Debug.WriteLine("Saving");
            Preferences.Set("break", breakTime);

            Preferences.Set("start", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startTime.Hours, startTime.Minutes, startTime.Seconds));
            Preferences.Set("end", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endTime.Hours, endTime.Minutes, endTime.Seconds));

            await Shell.Current.GoToAsync("///" + nameof(AboutPage));
        }
    }
}
