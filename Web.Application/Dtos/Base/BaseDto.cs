namespace Web.Application.Dtos.Base
{
    public class BaseDto
    {
        public string Language { get; set; } = "en";

        public int? TenantId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }
    }
}