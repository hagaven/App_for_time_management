using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("SubActivities")]
    public class SubItem
    {
        [PrimaryKey]
        public string ID { get; set; }

        [MaxLength(200)]
        public string Text { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsDone { get; set; }

        [ForeignKey(typeof(Item))]
        public string ParentID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Item ParentActivity { get; set; }
    }
}
