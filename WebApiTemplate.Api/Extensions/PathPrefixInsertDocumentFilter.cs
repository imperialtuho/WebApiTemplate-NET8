using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiTemplate.Api.Extensions
{
    /// <summary>
    /// A Swagger document filter that modifies API endpoint paths by inserting a prefix
    /// and optionally hides the API documentation.
    /// </summary>
    /// <remarks>
    /// If <paramref name="isExposed"/> is <c>false</c>, the API endpoints and schemas will be removed from the Swagger documentation.
    /// If <paramref name="isExposed"/> is <c>true</c>, the specified <paramref name="prefix"/> is added to all API paths.
    /// </remarks>
    /// <param name="prefix">The prefix to insert before API paths.</param>
    /// <param name="isExposed">Determines whether the API documentation should be exposed.</param>
    public class PathPrefixInsertDocumentFilter(string prefix, bool isExposed) : IDocumentFilter
    {
        /// <summary>
        /// Applies the document filter to modify or remove API paths in the Swagger document.
        /// </summary>
        /// <param name="swaggerDoc">The OpenAPI document being modified.</param>
        /// <param name="context">The document filter context.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (!isExposed)
            {
                // Remove all API paths (endpoints) from Swagger documentation
                List<KeyValuePair<string, OpenApiPathItem>> paths = [.. swaggerDoc.Paths];
                paths.ForEach(p => swaggerDoc.Paths.Remove(p.Key));

                // Remove all API schemas (models) from Swagger documentation
                List<KeyValuePair<string, OpenApiSchema>> schemas = [.. swaggerDoc.Components.Schemas];
                schemas.ForEach(s => swaggerDoc.Components.Schemas.Remove(s.Key));
            }
            else
            {
                // Add prefix to API paths
                List<string> pathKeys = [.. swaggerDoc.Paths.Keys];
                foreach (string path in pathKeys)
                {
                    OpenApiPathItem pathToChange = swaggerDoc.Paths[path];
                    swaggerDoc.Paths.Remove(path);
                    swaggerDoc.Paths.Add($"{prefix}{path}", pathToChange);
                }
            }
        }
    }
}