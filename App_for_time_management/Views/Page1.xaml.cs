﻿using App_for_time_management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_for_time_management.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        ScheduleViewModel scheduleView;
        public Page1()
        {
            InitializeComponent();
            BindingContext = scheduleView = new ScheduleViewModel();

        }
    }
}