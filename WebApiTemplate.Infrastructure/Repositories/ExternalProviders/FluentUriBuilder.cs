using System.Collections.Specialized;
using System.Web;

namespace WebApiTemplate.Infrastructure.Repositories.ExternalProviders
{
    /// <summary>
    /// A fluent builder for constructing URIs with dynamic path segments and query parameters.
    /// This class provides a flexible and readable way to build URIs step by step by adding path segments
    /// and query parameters, with support for preventing duplicate path segments and handling URL encoding automatically.
    /// </summary>
    public class FluentUriBuilder
    {
        private readonly HashSet<string> _pathSegments = []; // Use HashSet to prevent duplicate segments
        private readonly NameValueCollection _queryParams = []; // To store query parameters
        private readonly string _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentUriBuilder"/> class.
        /// This constructor initializes the base URI that serves as the foundation for further URI construction.
        /// </summary>
        /// <param name="baseUri">The base URI to build upon. Cannot be null or empty.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="baseUri"/> is null or empty.</exception>
        /// <remarks>
        /// The base URI should be a valid URL without a trailing slash. Any additional segments and query parameters
        /// can be appended using the methods provided by this builder.
        /// </remarks>
        public FluentUriBuilder(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                throw new ArgumentException("Base URI cannot be null or empty.", nameof(baseUri));
            }

            _baseUri = baseUri.TrimEnd('/'); // Ensure the base URI does not end with a slash
        }

        /// <summary>
        /// Gets the fully constructed <see cref="Uri"/> with appended path segments and query parameters.
        /// </summary>
        /// <returns>The fully constructed URI, including path segments and query parameters.</returns>
        /// <remarks>
        /// This property constructs the final URI by combining the base URI, the path segments added with
        /// <see cref="AppendPath"/> or <see cref="AppendPaths"/>, and any query parameters added with
        /// <see cref="AddQueryParam"/> or <see cref="AddQueryParams"/>.
        /// The resulting URI is URL-encoded as necessary.
        /// </remarks>
        public Uri Uri
        {
            get
            {
                // Combine the base URI with the appended path segments
                string? combinedPath = string.Join("/", _pathSegments);
                string? fullPath = $"{_baseUri}/{combinedPath}".TrimEnd('/'); // Ensure no trailing slash

                // Construct the query string by URL-encoding the query parameters
                IEnumerable<string>? pairs = from key in _queryParams.AllKeys
                                             from value in _queryParams.GetValues(key)!
                                             select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
                string? queryString = string.Join("&", pairs);

                // Return the final URI with path and query string
                return new Uri($"{fullPath}?{queryString}".TrimEnd('?')); // Combine full path with query string
            }
        }

        /// <summary>
        /// Adds or updates a single query string parameter.
        /// If the parameter already exists, its value will be updated; otherwise, it will be added.
        /// </summary>
        /// <param name="name">The query parameter name.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method allows the addition or updating of a single query parameter. If a parameter with the same name
        /// already exists, its value will be overwritten with the new value provided.
        /// </remarks>
        public FluentUriBuilder AddQueryParam(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Query parameter name cannot be null or empty.", nameof(name));
            }

            if (value != null)
            {
                _queryParams[name] = value.ToString(); // Overwrite if exists
            }

            return this;
        }

        /// <summary>
        /// Adds or updates multiple query string parameters.
        /// </summary>
        /// <param name="parameters">A dictionary containing the query parameter names and values.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method allows you to add multiple query parameters at once by providing a dictionary of parameter names and values.
        /// It automatically prevents duplicates by using the <see cref="AddQueryParam"/> method for each parameter.
        /// </remarks>
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
        /// This method adds a segment to the URI path while ensuring that duplicates are avoided.
        /// </summary>
        /// <param name="segment">The path segment to append to the URI.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// Path segments are appended to the base URI in the order they are added. Duplicate segments are ignored,
        /// ensuring that the URI remains clean and does not contain redundant segments.
        /// </remarks>
        public FluentUriBuilder AppendPath(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                _pathSegments.Add(segment.Trim('/')); // Add to HashSet to avoid duplicates
            }

            return this;
        }

        /// <summary>
        /// Appends multiple path segments to the URI.
        /// This method adds a list of segments to the URI path while ensuring that duplicates are avoided.
        /// </summary>
        /// <param name="segments">The path segments to append to the URI.</param>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method allows you to append multiple segments at once. Duplicate segments are ignored, and
        /// all valid segments will be added to the URI.
        /// </remarks>
        public FluentUriBuilder AppendPaths(IEnumerable<string> segments)
        {
            if (segments != null)
            {
                foreach (string segment in segments.Where(s => !string.IsNullOrEmpty(s)))
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
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method removes the specified query parameter from the URI. If the parameter does not exist, no action is taken.
        /// </remarks>
        public FluentUriBuilder RemoveQueryParam(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return this;
            }

            _queryParams.Remove(name);

            return this;
        }

        /// <summary>
        /// Clears all query parameters.
        /// </summary>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method removes all query parameters from the URI. It can be used if you want to start fresh with new parameters.
        /// </remarks>
        public FluentUriBuilder ClearQueryParams()
        {
            _queryParams.Clear();

            return this;
        }

        /// <summary>
        /// Clears all path segments.
        /// </summary>
        /// <returns>The same <see cref="FluentUriBuilder"/> instance, allowing for method chaining.</returns>
        /// <remarks>
        /// This method removes all path segments from the URI. It can be used if you want to start fresh with new path segments.
        /// </remarks>
        public FluentUriBuilder ClearPathSegments()
        {
            _pathSegments.Clear();

            return this;
        }
    }
}