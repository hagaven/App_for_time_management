using SQLite;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("ActivityNotes")]
    public class ActivityNote
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Content { get; set; }

        [ForeignKey(typeof(Item))]
        public string ParentID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Item ParentActivity { get; set; }
    }
}
