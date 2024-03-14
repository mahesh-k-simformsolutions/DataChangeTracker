using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataChangeTracker.Data.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product>  Products { get; set; }
        public bool IsDeleted { get; internal set; }
    }
}
