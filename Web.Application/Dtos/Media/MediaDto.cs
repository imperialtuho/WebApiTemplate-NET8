using Web.Application.Dtos.Base;

namespace Web.Application.Dtos.Media
{
    public class MediaDto : BaseDto
    {
        public string Id { get; set; }

        public string? AltText { get; set; }

        public string? FileName { get; set; }

        public bool IsDisplayed { get; set; }

        public string? ImageUrl { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string Type { get; set; }
    }
}