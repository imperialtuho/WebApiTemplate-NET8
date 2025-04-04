using System.ComponentModel.DataAnnotations;

namespace WebApiTemplate.Domain.Entities
{
    public class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; set; }

        public string Language { get; set; } = "en";

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public int? TenantId { get; set; }

        public bool IsDeleted { get; set; }
    }
}