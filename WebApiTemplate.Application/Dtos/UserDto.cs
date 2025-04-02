using WebApiTemplate.Application.Dtos.Base;

namespace WebApiTemplate.Application.Dtos
{
    public class UserDto : BaseDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}