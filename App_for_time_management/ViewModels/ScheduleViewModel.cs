using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App_for_time_management.ViewModels
{
    class ScheduleViewModel:BaseViewModel
    {
        private Item _selectedItem;
        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command<Item> ItemTapped { get; }

        public ScheduleViewModel()
        {
            Title = "Harmonogram";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);

            
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.Database.GetItemsAsync(true);
                List<Item> temporaryList = new List<Item>();
                foreach (var item in items)
                {
                    if (item.TimeSensitive && (item.DeadlineDate.Date == DateTime.Now.Date))
                    {
                        Items.Add(item);


                    }
                    else
                    {
                        temporaryList.Add(item);
                    }
                    
                }
                temporaryList.Sort((p, q) =>
                {
                    TimeSpan pTime = p.DeadlineDate.Subtract(DateTime.Now);
                    TimeSpan qTime = q.DeadlineDate.Subtract(DateTime.Now);
                    double pMultiplier = 1.0;
                    double qMultiplier = 1.0;
                    switch (p.Eisenhower)
                    {
                        case "Ważne i pilne":
                            {
                                pMultiplier = 1.5;
                                break;
                            }
                        case "Ważne i niepilne":
                            {
                                pMultiplier = 1.0;
                                break;
                            }
                        case "Nieważne i pilne":
                            {
                                pMultiplier = 1.25;
                                break;
                            }
                        case "Nieważne i niepilne":
                            {
                                pMultiplier = 0.5;
                                break;
                            }
                        default:
                            {
                                qMultiplier = 1.0;
                                break;
                            }
                    }
                    switch (q.Eisenhower)
                    {
                        case "Ważne i pilne":
                            {
                                qMultiplier = 1.5;
                                break;
                            }
                        case "Ważne i niepilne":
                            {
                                qMultiplier = 1.0;
                                break;
                            }
                        case "Nieważne i pilne":
                            {
                                qMultiplier = 1.25;
                                break;
                            }
                        case "Nieważne i niepilne":
                            {
                                qMultiplier = 0.5;
                                break;
                            }
                        default:
                            {
                                qMultiplier = 1.0;
                                break;
                            }
                    }
                    double pPriority = pTime.TotalHours * pMultiplier;
                    double qPriority = qTime.TotalHours * qMultiplier;
                    if (pPriority > qPriority)
                    {
                        return -1;
                    }
                    else if(pPriority < qPriority)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });
                foreach(var item in temporaryList)
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
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;


            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.ID.ToString()}");
        }

    }
}
