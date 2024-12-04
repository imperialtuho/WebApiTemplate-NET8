namespace Web.Domain.SharedKernel
{
    public class UserSession
    {
        public string UserId { set; get; }

        public List<string>? Roles { get; set; }

        public List<string>? Permissions { get; set; }

        public int? TenantId { get; set; }

        public string? Email { get; set; }
    }
}