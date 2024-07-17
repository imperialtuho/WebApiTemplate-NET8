namespace Web.Application.Configurations.Cache
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        Task<List<T>> GetDocumentsAsync<T>(List<string> keys, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        Task<string> GetDocumentStringAsync(string key, Guid correlationId, string? moduleName = null);

        Task InsertAsync(string key, object data, Guid correlationId, string? moduleName = null, params object[] args);

        Task InsertAsync(string key, object data, TimeSpan slidingExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        Task InsertAsync(string key, object data, DateTime absoluteExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        Task RemoveAsync(string key, Guid correlationId, string? moduleName = null, params object[] args);

        Task RemoveByKeyStartAsync(string keyStart, Guid correlationId, string? moduleName = null);

        Task FlushDocuments(Guid correlationId, string? moduleName = null);

        Task<bool> RemoveByPatternAsync(string pattern, Guid correlationId, string? moduleName = null);
    }
}