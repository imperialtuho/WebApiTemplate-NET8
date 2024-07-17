namespace Web.Domain.Entities
{
    public class Blog : BaseEntity<string>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string? ParentId { get; set; }

        public string ImageUrl { get; set; }

        public string Url { get; set; }
    }
}