using App_for_time_management.Services;
using App_for_time_management.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_for_time_management
{
    public partial class App : Application
    {
        static DataStore database;
        public static DataStore Database
        {
            get
            {
                if (database == null)
                {
                    database = new DataStore();

                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();

            DependencyService.Register<DataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
