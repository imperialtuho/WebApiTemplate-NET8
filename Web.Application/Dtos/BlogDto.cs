using System.ComponentModel.DataAnnotations;

namespace Web.Application.Dtos
{
    public class BlogDto
    {
        [Required]
        public string Id { get; set; }

        public string Title { get; set; }

        public string? Content { get; set; }

        public string? ParentId { get; set; }

        public string? ImageUrl { get; set; }

        public string? Url { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}