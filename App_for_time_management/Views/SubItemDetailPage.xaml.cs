using App_for_time_management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_for_time_management.Views
{
    
    public partial class SubItemDetailPage : ContentPage
    {
        public SubItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new SubItemDetailViewModel();
        }
    }
}