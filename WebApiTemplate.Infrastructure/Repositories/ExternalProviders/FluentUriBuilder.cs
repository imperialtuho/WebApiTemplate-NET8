using System.Collections.Specialized;
using System.Web;

namespace WebApiTemplate.Infrastructure.Repositories.ExternalProviders
{
    public class FluentUriBuilder
    {
        private readonly HashSet<string> _pathSegments = []; // Use HashSet to prevent duplicate segments
        private readonly NameValueCollection _queryParams = [];
        private readonly string _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentUriBuilder"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI to build upon.</param>
        public FluentUriBuilder(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new ArgumentException("Base URI cannot be null or empty.", nameof(baseUri));

            _baseUri = baseUri.TrimEnd('/');
        }

        /// <summary>
        /// Gets the fully constructed <see cref="Uri"/>.
        /// </summary>
        public Uri Uri
        {
            get
            {
                // Combine base URI and path segments
                string? combinedPath = string.Join("/", _pathSegments);
                string? fullPath = $"{_baseUri}/{combinedPath}".TrimEnd('/');

                // Build query string
                IEnumerable<string>? pairs = from key in _queryParams.AllKeys
                                             from value in _queryParams.GetValues(key)!
                                             select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
                string? queryString = string.Join("&", pairs);

                return new Uri($"{fullPath}?{queryString}".TrimEnd('?'));
            }
        }

        /// <summary>
        /// Adds or updates a single query string parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder AddQueryParam(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Query parameter name cannot be null or empty.", nameof(name));

            if (value != null)
                _queryParams[name] = value.ToString(); // Overwrite if exists

            return this;
        }

        /// <summary>
        /// Adds or updates multiple query string parameters.
        /// </summary>
        /// <param name="parameters">A dictionary containing parameter names and values.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder AddQueryParams(IDictionary<string, object> parameters)
        {
            if (parameters == null) return this;

            foreach (var param in parameters.Where(p => p.Value != null))
            {
                AddQueryParam(param.Key, param.Value); // Use AddQueryParam to prevent duplicates
            }

            return this;
        }

        /// <summary>
        /// Appends a single path segment to the URI.
        /// </summary>
        /// <param name="segment">The path segment to append.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder AppendPath(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
                _pathSegments.Add(segment.Trim('/')); // Add to HashSet to avoid duplicates

            return this;
        }

        /// <summary>
        /// Appends multiple path segments to the URI.
        /// </summary>
        /// <param name="segments">The path segments to append.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder AppendPaths(IEnumerable<string> segments)
        {
            if (segments != null)
            {
                foreach (var segment in segments.Where(s => !string.IsNullOrEmpty(s)))
                {
                    _pathSegments.Add(segment.Trim('/')); // Add to HashSet to avoid duplicates
                }
            }

            return this;
        }

        /// <summary>
        /// Removes a specific query parameter by name.
        /// </summary>
        /// <param name="name">The name of the query parameter to remove.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder RemoveQueryParam(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return this;

            _queryParams.Remove(name);
            return this;
        }

        /// <summary>
        /// Clears all query parameters.
        /// </summary>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder ClearQueryParams()
        {
            _queryParams.Clear();
            return this;
        }

        /// <summary>
        /// Clears all path segments.
        /// </summary>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance.</returns>
        public FluentUriBuilder ClearPathSegments()
        {
            _pathSegments.Clear();
            return this;
        }
    }
}