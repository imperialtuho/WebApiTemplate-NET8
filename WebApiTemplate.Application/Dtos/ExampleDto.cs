using WebApiTemplate.Application.Dtos.Base;

namespace WebApiTemplate.Application.Dtos
{
    public class ExampleDto : BaseDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}