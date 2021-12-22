using App_for_time_management.ViewModels;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace App_for_time_management
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
            Routing.RegisterRoute(nameof(NewSubItemPage), typeof(NewSubItemPage));
            Routing.RegisterRoute(nameof(SubItemDetailPage), typeof(SubItemDetailPage));
        }

    }
}
