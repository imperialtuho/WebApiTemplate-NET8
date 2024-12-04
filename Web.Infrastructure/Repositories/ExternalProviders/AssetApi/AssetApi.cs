using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Web.Application.Dtos.Media;
using Web.Application.Interfaces.ExternalProviders;

namespace Web.Infrastructure.Repositories.ExternalProviders.AssetApi
{
    public class AssetApi(ILogger<WebApiClient> logger,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor) : WebApiClient(logger, httpClientFactory, httpContextAccessor), IAssetApi
    {
        public Task<IList<MediaDto>> GetMediaInformationByIdsAsync(IEnumerable<string> mediaIds)
        {
            throw new NotImplementedException();
        }
    }
}