namespace WebApiTemplate.Domain.Entities
{
    public class ExampleEntity : BaseEntity<string>
    {
        public string Name { get; set; }

        public string UserId { get; set; }
    }
}