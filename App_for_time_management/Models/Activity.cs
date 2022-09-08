using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("Activities")]
    public class Activity
    {
        [PrimaryKey]
        public string ID { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public DateTime DeadlineDate { get; set; }

        public TimeSpan DeadlineTime { get; set; }

        [MaxLength(100)]
        public string Eisenhower { get; set; }

        public bool TimeSensitive { get; set; }

        public DateTime AdditionDate { get; set; }

        public bool IsDone { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<SubActivity> SubActivity { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ActivityNote> ActivityNotes { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsCyclic { get; set; }

        public string CyclePeriod { get; set; }



    }
}