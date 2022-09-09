using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("SubActivities")]
    public class SubActivity
    {
        [PrimaryKey]
        public string ID { get; set; }

        [MaxLength(200)]
        public string Text { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsDone { get; set; }

        [ForeignKey(typeof(Activity))]
        public string ParentID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Activity ParentActivity { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<SubActivityNote> SubActivityNotes { get; set; }
    }
}
