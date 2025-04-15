namespace WebApiTemplate.Domain.Common
{
    public static class ApplicationPolicies
    {
        public const string Full = nameof(Full);
        public const string Read = nameof(Read);
        public const string Write = nameof(Write);
        public const string Special = nameof(Special);

        public static readonly List<string> DefaultPolicies = [Read, Write];
    }
}