namespace Web.Domain.Helpers
{
    public static class CheckingHelper
    {
        public static bool IsBase64String(string source)
        {
            var buffer = new Span<byte>(new byte[source.Length]);

            return Convert.TryFromBase64String(source, buffer, out _);
        }
    }
}