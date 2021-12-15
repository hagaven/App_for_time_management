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
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(200)]
        public string Text { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [ForeignKey(typeof(Item))]
        public int parentID { get; set; }

        [ManyToOne]
        public Item parentActivity { get; set; }
    }
}
