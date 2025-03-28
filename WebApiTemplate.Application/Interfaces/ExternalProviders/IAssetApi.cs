using WebApiTemplate.Application.Dtos.Media;

namespace WebApiTemplate.Application.Interfaces.ExternalProviders
{
    public interface IAssetApi
    {
        Task<IList<MediaDto>> GetMediaInformationByIdsAsync(IEnumerable<string> mediaIds);
    }
}