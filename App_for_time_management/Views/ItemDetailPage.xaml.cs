using App_for_time_management.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace App_for_time_management.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}