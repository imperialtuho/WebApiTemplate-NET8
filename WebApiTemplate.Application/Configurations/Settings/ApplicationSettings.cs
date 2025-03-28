namespace WebApiTemplate.Application.Configurations.Settings
{
    public class ApplicationSettings
    {
        public string? BaseUrl { get; set; }

        public IList<string>? AllowedRoles { get; set; }

        public IList<string>? AvailableClaimPolicies { get; set; }

        public string? ProviderName { get; set; }

        public string Password { get; set; }

        public bool IsProductionMode { get; set; }

        public string Kid { get; set; }
    }
}