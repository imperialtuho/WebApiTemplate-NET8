using System.ComponentModel.DataAnnotations;

namespace Web.Domain.Entities
{
    public class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; } = null!;

        public string ModifiedBy { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}