using App_for_time_management.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            AddItemCommand = new Command(OnAddItem);
        }

        public ICommand OpenWebCommand { get; }

        public Command AddItemCommand { get; }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }
    }
}