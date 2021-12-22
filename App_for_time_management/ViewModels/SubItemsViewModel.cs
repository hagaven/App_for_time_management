﻿using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    class SubItemsViewModel:BaseViewModel
    {
        private SubItem _selectedSubItem;
        public ObservableCollection<SubItem> SubItems { get; }
        public Command LoadItemsCommand { get; }
        public Command AddSubItemCommand { get; }
        public Command<SubItem> SubItemTapped { get; }
        public SubItemsViewModel()
        {
            Title = "Browse";
            SubItems = new ObservableCollection<SubItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadSubItemsCommand());

            SubItemTapped = new Command<SubItem>(OnSubItemSelected);

            AddSubItemCommand = new Command(OnAddSubItem);
        }

        private async Task ExecuteLoadSubItemsCommand()
        {
            IsBusy = true;

            try
            {
                SubItems.Clear();
                var subitems = await App.Database.GetSubItemsAsync(true);
                foreach (var item in subitems)
                {
                    SubItems.Add(item);
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSubItem = null;
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

        private async void OnAddSubItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewSubItemPage));
        }

        
        private async void OnSubItemSelected(SubItem subItem)
        {
            if (subItem == null)
                return;


            await Shell.Current.GoToAsync($"{nameof(SubItemDetailPage)}?{nameof(SubItemDetailViewModel.SubItemId)}={subItem.ID}");
        }
    }
}