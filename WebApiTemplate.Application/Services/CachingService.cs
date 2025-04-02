using WebApiTemplate.Application.Configurations.Cache;

namespace WebApiTemplate.Application.Services
{
    public class CachingService : ICachingService
    {
        public Task<T> GetAsync<T>(string key, Guid correlationId, string? moduleName = null, params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetDocumentsAsync<T>(List<string> keys, Guid correlationId, string? moduleName = null, params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDocumentStringAsync(string key, Guid correlationId, string? moduleName = null)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(string key, object data, Guid correlationId, string? moduleName = null, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(string key, object data, TimeSpan slidingExpiration, Guid correlationId, string? moduleName = null, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(string key, object data, DateTime absoluteExpiration, Guid correlationId, string? moduleName = null, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, Guid correlationId, string? moduleName = null, params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task RemoveByKeyStartAsync(string keyStart, Guid correlationId, string? moduleName = null)
        {
            throw new NotImplementedException();
        }

        public Task FlushDocuments(Guid correlationId, string? moduleName = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveByPatternAsync(string pattern, Guid correlationId, string? moduleName = null)
        {
            throw new NotImplementedException();
        }
    }
}