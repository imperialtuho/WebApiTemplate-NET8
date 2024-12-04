namespace Web.Application.Dtos.Author
{
    public class AuthorDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public int? TenantId { get; set; }
    }
}