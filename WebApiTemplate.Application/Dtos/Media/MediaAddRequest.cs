namespace WebApiTemplate.Application.Dtos.Media
{
    public class MediaAddRequest
    {
        public string? AltText { get; set; }

        public string? FileName { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string Type { get; set; }
    }
}