using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Activity _selectedItem;

        public ObservableCollection<Activity> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Activity> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Aktywności";
            Items = new ObservableCollection<Activity>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Activity>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                System.Collections.Generic.IEnumerable<Activity> items = await App.Database.GetItemsAsync(true);
                foreach (Activity item in items)
                {
                    Items.Add(item);
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
            SelectedItem = null;
        }

        public Activity SelectedItem
        {
            get => _selectedItem;
            set
            {
                _ = SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        private async void OnItemSelected(Activity item)
        {
            if (item == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.ID}");
        }
    }
}