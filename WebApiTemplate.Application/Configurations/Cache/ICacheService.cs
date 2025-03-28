namespace WebApiTemplate.Application.Configurations.Cache
{
    /// <summary>
    /// Defines a caching service for storing, retrieving, and managing data in a cache store.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a cached object asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the cached object.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache retrieval.</param>
        /// <returns>The cached object if found; otherwise, null.</returns>
        Task<T> GetAsync<T>(string key, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        /// <summary>
        /// Retrieves multiple cached objects asynchronously based on a list of keys.
        /// </summary>
        /// <typeparam name="T">The type of the cached objects.</typeparam>
        /// <param name="keys">The list of cache keys.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache retrieval.</param>
        /// <returns>A list of cached objects.</returns>
        Task<List<T>> GetDocumentsAsync<T>(List<string> keys, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        /// <summary>
        /// Retrieves a cached document as a string asynchronously.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>The cached document as a string.</returns>
        Task<string> GetDocumentStringAsync(string key, Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Inserts an object into the cache with default expiration settings.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        Task InsertAsync(string key, object data, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Inserts an object into the cache with a specified sliding expiration time.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="slidingExpiration">The duration the cache entry remains in memory before expiration.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        Task InsertAsync(string key, object data, TimeSpan slidingExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Inserts an object into the cache with an absolute expiration time.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="absoluteExpiration">The date and time when the cache entry expires.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        Task InsertAsync(string key, object data, DateTime absoluteExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Removes a specific cache entry asynchronously.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache removal.</param>
        Task RemoveAsync(string key, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Removes all cache entries that start with the specified key prefix asynchronously.
        /// </summary>
        /// <param name="keyStart">The key prefix to match for removal.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        Task RemoveByKeyStartAsync(string keyStart, Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Clears all cached documents asynchronously.
        /// </summary>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        Task FlushDocuments(Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Removes all cache entries that match a specific pattern asynchronously.
        /// </summary>
        /// <param name="pattern">The regex pattern to match keys for removal.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>True if at least one entry was removed; otherwise, false.</returns>
        Task<bool> RemoveByPatternAsync(string pattern, Guid correlationId, string? moduleName = null);
    }
}