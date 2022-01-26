using App_for_time_management.Models;
using App_for_time_management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private TimeSpan end;
        private TimeSpan break_duration;

        public ScheduleViewModel()
        {
            Title = "Harmonogram";
            Items = new ObservableCollection<ScheduledItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);
            

            
        }

        public async Task ExecuteLoadItemsCommand()
        {
            Debug.WriteLine("ex load");
            IsBusy = true;
            planned = new TimeSpan();
            DateTime startDT = Preferences.Get("start", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0));
            DateTime endDT = Preferences.Get("end", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 0, 0));
            int breakDT = Preferences.Get("break", 10);
            start = new TimeSpan(startDT.Hour, startDT.Minute, startDT.Second);
            end = new TimeSpan(endDT.Hour, endDT.Minute, endDT.Second);
            break_duration = new TimeSpan(0, breakDT, 0);
            try
            {
                Items.Clear();
                var items = (await App.Database.GetItemsAsync(true)).ToList();
                List<Item> temporaryList = new List<Item>();
                foreach (var item in items)
                {
                    DateTime minimalStartTime = item.DeadlineDate.Subtract(item.Duration);
                    if (item.TimeSensitive && (minimalStartTime.Date == DateTime.Now.Date))
                    {
                        ScheduledItem scheduled = new ScheduledItem
                        {
                            StartTime = new TimeSpan(minimalStartTime.Hour,minimalStartTime.Minute,0),
                            Scheduled = item
                            
                        };
                        Items.Add(scheduled);
                        planned = planned.Add(scheduled.Scheduled.Duration);
                        start = start.Add(scheduled.Scheduled.Duration).Add(break_duration);
                        foreach(var sub in item.SubActivity)
                        {
                            scheduled.SubActivities.Add(sub);
                        }
                        if (item.IsCyclic)
                        {
                            DateTime futureDate;
                            switch (item.CyclePeriod)
                            {
                                case "Codziennie":
                                    futureDate = item.DeadlineDate.AddDays(1);
                                    break;
                                case "Co tydzień":
                                    futureDate = item.DeadlineDate.AddDays(7);
                                    break;
                                case "Co miesiąc":
                                    futureDate = item.DeadlineDate.AddMonths(1);
                                    break;
                                case "Co rok":
                                    futureDate = item.DeadlineDate.AddYears(1);
                                    break;
                                default:
                                    futureDate = item.DeadlineDate.AddYears(10);
                                    break;
                            }
                            Item repeated = new Item()
                            {
                                ID = Guid.NewGuid().ToString(),
                                Name = item.Name,
                                Description = item.Description,
                                DeadlineDate = futureDate,
                                DeadlineTime = item.DeadlineTime,
                                Eisenhower = item.Eisenhower,
                                TimeSensitive = item.TimeSensitive,
                                IsDone = false,
                                AdditionDate = DateTime.Now,
                                Duration = item.Duration,
                                IsCyclic = item.IsCyclic,
                                CyclePeriod = item.CyclePeriod
                            };
                            
                            
                            if (!items.Exists(i => repeated.Name.Equals(i.Name) && repeated.DeadlineDate.Equals(i.DeadlineDate)))
                            {
                                App.Database.AddItemAsync(repeated);
                            }
                        }



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
                    if ((planned.Ticks / end.Ticks) > 0.6)
                    {
                        break;
                    }
                    if (!item.IsDone)
                    {
                        int subActivityCount = item.SubActivity.Count;
                        ObservableCollection<SubItem> subItems = new ObservableCollection<SubItem>();
                        if(!(subActivityCount == 0) && !(subActivityCount == 1))
                        {
                            int daysLeft = item.DeadlineDate.Subtract(DateTime.Now).Days;
                            int numberOfSubActivities = subActivityCount;
                            if (daysLeft > 0)
                            {
                                numberOfSubActivities = (int)Math.Ceiling((double)(subActivityCount / daysLeft));
                            }
                            int i = 0;
                            foreach (SubItem subItem in item.SubActivity)
                            {
                                if (!(i < numberOfSubActivities))
                                {
                                    break;
                                }
                                if (!item.SubActivity[i].IsDone)
                                {
                                    subItems.Add(item.SubActivity[i]);
                                    i++;
                                }
                            }
                        }
                        TimeSpan subActivitiesDuration = new TimeSpan();
                        foreach (SubItem sub in subItems)
                        {
                            subActivitiesDuration = subActivitiesDuration.Add(sub.Duration);
                        }
                        item.Duration = subItems.Count > 0 ? subActivitiesDuration : item.Duration;
                        foreach (var i in Items)
                        {
                            if ((i.StartTime < start)&&(start < i.Scheduled.DeadlineTime))
                            {
                                start = i.Scheduled.DeadlineTime.Add(break_duration);
                            }
                        }
                        item.DeadlineTime = start.Add(item.Duration);
                        ScheduledItem scheduled = new ScheduledItem
                        {
                            StartTime = start,
                            Scheduled = item,
                            SubActivities = subItems
                        };
                        Items.Add(scheduled);
                        planned = planned.Add(scheduled.Scheduled.Duration);
                        start = start.Add(scheduled.Scheduled.Duration).Add(break_duration);
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
                //OnItemSelected(value);
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
