namespace Web.Application.Configurations.Settings
{
    public class RedisSettings
    {
        public string Endpoints { get; set; }

        public string ServiceName { get; set; }

        public string Password { get; set; }

        public bool AbortOnConnectFail { get; set; }

        public bool AllowAdmin { get; set; }

        public int ExpiryTimeInHours { get; set; }

        public string CommandFlushData { get; set; }
    }
}