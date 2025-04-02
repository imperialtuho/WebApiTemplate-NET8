namespace WebApiTemplate.Application.Configurations.Cache
{
    /// <summary>
    /// Defines a caching service for storing, retrieving, and managing data in a cache store.
    /// </summary>
    /// <remarks>
    /// This interface provides various methods to interact with the cache, including retrieving cached objects, inserting new cache entries with various expiration strategies,
    /// and removing cache entries. Cache operations are tracked via correlation IDs to ensure traceability across requests. The service supports both single-object and batch cache retrieval.
    /// </remarks>
    public interface ICachingService
    {
        /// <summary>
        /// Asynchronously retrieves a cached object.
        /// </summary>
        /// <typeparam name="T">The type of the cached object.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache retrieval.</param>
        /// <returns>A task representing the asynchronous operation, with the cached object or null if not found.</returns>
        /// <remarks>
        /// This method checks the cache for the given key and returns the corresponding object if it exists.
        /// If no matching entry is found, it will return null. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task<T> GetAsync<T>(string key, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        /// <summary>
        /// Asynchronously retrieves multiple cached objects.
        /// </summary>
        /// <typeparam name="T">The type of the cached objects.</typeparam>
        /// <param name="keys">The list of cache keys.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache retrieval.</param>
        /// <returns>A task representing the asynchronous operation, with a list of cached objects.</returns>
        /// <remarks>
        /// This method checks the cache for each key in the provided list and returns a list of matching objects.
        /// If any key does not have a cached entry, it will return null for that key.
        /// </remarks>
        Task<List<T>> GetDocumentsAsync<T>(List<string> keys, Guid correlationId, string? moduleName = null, params object[] args) where T : class;

        /// <summary>
        /// Asynchronously retrieves a cached document as a string.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>A task representing the asynchronous operation, with the cached document as a string.</returns>
        /// <remarks>
        /// This method retrieves a document stored as a string in the cache for the given key. It returns the string if found or null if the entry does not exist.
        /// </remarks>
        Task<string> GetDocumentStringAsync(string key, Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Asynchronously inserts an object into the cache with default expiration settings.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method inserts the provided data into the cache with the default expiration policy (e.g., a fixed time to live).
        /// The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task InsertAsync(string key, object data, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Asynchronously inserts an object into the cache with a specified sliding expiration time.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="slidingExpiration">The duration the cache entry remains in memory before expiration.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method inserts the provided data into the cache with a sliding expiration policy, meaning the cache entry will expire after a certain period
        /// of inactivity. The expiration time will reset with each cache access. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task InsertAsync(string key, object data, TimeSpan slidingExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Asynchronously inserts an object into the cache with an absolute expiration time.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="absoluteExpiration">The date and time when the cache entry expires.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache insertion.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method inserts the provided data into the cache with an absolute expiration policy, meaning the cache entry will expire at the specified time.
        /// The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task InsertAsync(string key, object data, DateTime absoluteExpiration, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Asynchronously removes a specific cache entry.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <param name="args">Additional arguments for cache removal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method removes the cache entry for the given key. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task RemoveAsync(string key, Guid correlationId, string? moduleName = null, params object[] args);

        /// <summary>
        /// Asynchronously removes all cache entries that start with the specified key prefix.
        /// </summary>
        /// <param name="keyStart">The key prefix to match for removal.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method removes all cache entries whose keys start with the given prefix. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task RemoveByKeyStartAsync(string keyStart, Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Asynchronously clears all cached documents.
        /// </summary>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method removes all cached documents from the cache store. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task FlushDocuments(Guid correlationId, string? moduleName = null);

        /// <summary>
        /// Asynchronously removes all cache entries that match a specific pattern.
        /// </summary>
        /// <param name="pattern">The regex pattern to match keys for removal.</param>
        /// <param name="correlationId">The correlation ID for tracking the request.</param>
        /// <param name="moduleName">Optional module name for categorization.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating whether any entries were removed.</returns>
        /// <remarks>
        /// This method removes all cache entries whose keys match the specified pattern. The operation is tracked using the provided correlation ID.
        /// </remarks>
        Task<bool> RemoveByPatternAsync(string pattern, Guid correlationId, string? moduleName = null);
    }
}