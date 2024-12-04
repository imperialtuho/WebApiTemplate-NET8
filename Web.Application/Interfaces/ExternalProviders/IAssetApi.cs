using Web.Application.Dtos.Media;

namespace Web.Application.Interfaces.ExternalProviders
{
    public interface IAssetApi
    {
        Task<IList<MediaDto>> GetMediaInformationByIdsAsync(IEnumerable<string> mediaIds);
    }
}