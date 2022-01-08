using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace App_for_time_management.Models
{
    public class ScheduledItem
    {
        public Item Scheduled { get; set; }
        public TimeSpan StartTime { get; set; }

        public ObservableCollection<SubItem> SubActivities { get; set; }
    }
}
