using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataChangeTracker.Data.Models.Tracking
{
    public class Audit 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Operation { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public IEnumerable<AuditEntry> Changes { get; set; }
        public DateTime? ChangeDate { get; set; }
        [ForeignKey("ChangedBy")]
        public int ChangedById { get; set; }
        public User ChangedBy { get; set; }
    }

    //namespace UnNormalizedTable
    //{
    //    public class Audit
    //    {
    //        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //        [Key]
    //        public int Id { get; set; }
    //        public string Operation { get; set; }
    //        public string EntityName { get; set; }
    //        public int EntityId { get; set; }
    //        public DateTime? ChangeDate { get; set; }
    //        [ForeignKey("ChangedBy")]
    //        public int ChangedById { get; set; }
    //        public User ChangedBy { get; set; }

    //        public string? FieldName { get; set; }
    //        public string? OldValue { get; set; }
    //        public string? NewValue { get; set; }
    //    }
    //}
}
