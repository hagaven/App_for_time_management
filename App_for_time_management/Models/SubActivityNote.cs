using SQLite;
using SQLiteNetExtensions.Attributes;

namespace App_for_time_management.Models
{
    [Table("SubActivityNotes")]
    public class SubActivityNote
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Content { get; set; }

        [ForeignKey(typeof(SubActivity))]
        public string ParentID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public SubActivity ParentActivity { get; set; }
    }
}
