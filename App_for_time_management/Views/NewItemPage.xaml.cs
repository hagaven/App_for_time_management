using App_for_time_management.Models;
using App_for_time_management.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_for_time_management.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
            
        }

        private void DurationMinutesSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string msg = e.NewValue.ToString();
            this.durCntrl2.Text = msg;
        }
        private void DurationHoursSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string msg = e.NewValue.ToString();
            this.durCntrl1.Text = msg;

        }
    }
}