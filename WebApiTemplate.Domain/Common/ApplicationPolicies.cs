namespace WebApiTemplate.Domain.Common
{
    public static class ApplicationPolicies
    {
        public const string Super = nameof(Super);
        public const string Read = nameof(Read);
        public const string Write = nameof(Write);

        public static readonly List<string> DefaultPolicies = [Read, Write];
    }
}