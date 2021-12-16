using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("Activities")]
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(200)]
        public string Text { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public DateTime DeadlineDate { get; set; }

        public TimeSpan DeadlineTime { get; set; }

        [MaxLength(100)]
        public string Eisenhower { get; set; }

        public bool TimeSensitive { get; set; }

        public DateTime AdditionDate { get; set; }

        public bool IsDone { get; set; }

        [OneToMany]
        public List<SubItem> SubActivity { get; set; }

        public TimeSpan Duration { get; set; }



    }
}