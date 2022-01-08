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
            

            
        }

        public async Task ExecuteLoadItemsCommand()
        {
            Debug.WriteLine("ex load");
            IsBusy = true;
            planned = new TimeSpan();
            start = new TimeSpan(8, 0, 0);
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
                            Scheduled = item,
                            
                        };
                        Items.Add(scheduled);
                        planned = planned.Add(scheduled.Scheduled.Duration);
                        start = start.Add(scheduled.Scheduled.Duration).Add(new TimeSpan(0,15,0));
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
                            Item powtorzony = new Item()
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
                    if (planned.CompareTo(new TimeSpan(9, 10, 0)) > 0)
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
                        ScheduledItem scheduled = new ScheduledItem
                        {
                            StartTime = start,
                            Scheduled = item,
                            SubActivities = subItems
                        };
                        Items.Add(scheduled);
                        TimeSpan subActivitiesDuration = new TimeSpan();
                        foreach (SubItem sub in scheduled.SubActivities)
                        {
                            subActivitiesDuration = subActivitiesDuration.Add(sub.Duration);
                        }
                        planned = scheduled.SubActivities.Count > 0 ? planned.Add(subActivitiesDuration) : planned.Add(scheduled.Scheduled.Duration);

                        start = scheduled.SubActivities.Count > 0 ? start.Add(subActivitiesDuration).Add(new TimeSpan(0, 15, 0))  : start.Add(scheduled.Scheduled.Duration).Add(new TimeSpan(0, 15, 0));
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
