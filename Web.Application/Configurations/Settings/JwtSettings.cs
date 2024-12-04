namespace Web.Application.Configurations.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public bool EnableIdentityUrl { get; set; }

        public string Kid { get; set; }
    }
}