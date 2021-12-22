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
    public class ScheduleViewModel:BaseViewModel
    {
        private Item _selectedItem;
        public ObservableCollection<ScheduledItem> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command<Item> ItemTapped { get; }
        private TimeSpan planned;
        private TimeSpan start;

        public ScheduleViewModel()
        {
            Title = "Harmonogram";
            Items = new ObservableCollection<ScheduledItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);
            planned = new TimeSpan();
            start = new TimeSpan(8, 0, 0);

            
        }

        public async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                IEnumerable<Item> items = await App.Database.GetItemsAsync(true);
                List<Item> temporaryList = new List<Item>();
                foreach (var item in items)
                {
                    if (item.TimeSensitive && (item.DeadlineDate.Date == DateTime.Now.Date))
                    {
                        ScheduledItem scheduled = new ScheduledItem
                        {
                            StartTime = start,
                            Scheduled = item
                        };
                        Items.Add(scheduled);
                        planned = planned.Add(scheduled.Scheduled.Duration);
                        start = start.Add(scheduled.Scheduled.Duration).Add(new TimeSpan(0,15,0));




                    }
                    else if(!item.TimeSensitive)
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
                    return pPriority > qPriority ? -1 : pPriority < qPriority ? 1 : 0;
                });
                foreach(var item in temporaryList)
                {
                    if (planned.CompareTo(new TimeSpan(9, 10, 0)) > 0)
                    {
                        break;
                    }
                    if (!item.IsDone)
                    {
                        ScheduledItem scheduled = new ScheduledItem
                        {
                            StartTime = start,
                            Scheduled = item
                        };
                        Items.Add(scheduled);
                        planned = planned.Add(scheduled.Scheduled.Duration);
                        start = start.Add(scheduled.Scheduled.Duration).Add(new TimeSpan(0, 15, 0));
                    }
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

        private async void OnItemSelected(Item item)
        {
            if (item == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.ID}");
        }

    }
}
