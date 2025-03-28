using WebApiTemplate.Application.Dtos.Author;

namespace WebApiTemplate.Application.Interfaces.ExternalProviders
{
    public interface IIdentityApi
    {
        Task<AuthorDto?> GetUserByIdAsync(string id);

        Task<IList<AuthorDto>?> GetUserByIdsAsync(IList<string> ids);
    }
}