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
    
    public partial class SchedulePage : ContentPage
    {
        private readonly ScheduleViewModel _scheduleView;
        public SchedulePage()
        {
            InitializeComponent();
            BindingContext = _scheduleView = new ScheduleViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _scheduleView.OnAppearing();
        }
    }
}