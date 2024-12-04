using System.ComponentModel.DataAnnotations;

namespace Web.Domain.Entities
{
    public class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; set; }

        public string Language { get; set; } = "en";

        /// <summary>
        /// Route path.
        /// </summary>
        public string? UrlPath { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public int? TenantId { get; set; }

        public bool IsDeleted { get; set; }
    }
}