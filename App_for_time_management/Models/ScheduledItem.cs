using System;
using System.Collections.ObjectModel;

namespace App_for_time_management.Models
{
    public class ScheduledItem
    {
        public Activity Scheduled { get; set; }
        public TimeSpan StartTime { get; set; }

        public ObservableCollection<SubActivity> SubActivities { get; set; }
    }
}
