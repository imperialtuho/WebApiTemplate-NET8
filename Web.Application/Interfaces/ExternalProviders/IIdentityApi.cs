using Web.Application.Dtos.Author;

namespace Web.Application.Interfaces.ExternalProviders
{
    public interface IIdentityApi
    {
        Task<AuthorDto?> GetUserByIdAsync(string id);

        Task<IList<AuthorDto>?> GetUserByIdsAsync(IList<string> ids);
    }
}