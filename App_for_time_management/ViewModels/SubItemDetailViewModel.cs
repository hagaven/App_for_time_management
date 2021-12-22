using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using App_for_time_management.Models;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    [QueryProperty(nameof(SubItemId), nameof(SubItemId))]
    class SubItemDetailViewModel:BaseViewModel
    {
        private string subItemId;
        private string text;
        private string description;
        private TimeSpan duration;
        private SubItem subItem;
        private string activityTitle;

        public SubItemDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            DoneCommand = new Command(OnDone);
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
        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        public string SubItemId
        {
            get
            {
                return subItemId;
            }
            set
            {
                subItemId = value;
                LoadSubItemId(value);
            }
        }

        public string ActivityTitle
        {
            get => subItem.ParentActivity.Text;
            set => SetProperty(ref activityTitle, value);
        }
        public async void LoadSubItemId(string subItemId)
        {
            try
            {
                var t = DataStore.GetSubItemAsync(subItemId);

                subItem = await t;
                Id = subItem.ID;
                Text = subItem.Text;
                Description = subItem.Description;
                Duration = subItem.Duration;
                ActivityTitle = subItem.ParentActivity.Text;
                

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
            await DataStore.DeleteItemAsync(Id);
            await Shell.Current.GoToAsync("..");

        }

        public Command DoneCommand { get; }

        private async void OnDone()
        {
            subItem.IsDone = true;
            await DataStore.UpdateSubItemAsync(subItem);
            await Shell.Current.GoToAsync("..");
        }

    }
}
