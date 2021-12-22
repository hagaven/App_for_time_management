using App_for_time_management.Models;
using App_for_time_management.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_for_time_management.Views
{
    public partial class NewSubItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewSubItemPage()
        {
            InitializeComponent();
            BindingContext = new NewSubItemViewModel();

        }

        private void DurationMinutesSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string msg = Math.Round(e.NewValue).ToString();
            durCntrl2.Text = msg;
        }
        private void DurationHoursSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string msg = Math.Round(e.NewValue).ToString();
            durCntrl1.Text = msg;

        }
    }
}