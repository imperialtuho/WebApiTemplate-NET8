using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiTemplate.Api.Extensions
{
    /// <summary>
    /// Adds a common header parameter (`TenantIdentify`) to all API operations in Swagger.
    /// </summary>
    /// <remarks>
    /// This filter ensures that the `TenantIdentify` header is included in API documentation.
    /// </remarks>
    public class AddCommonParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the operation filter by adding a custom header parameter.
        /// </summary>
        /// <param name="operation">The OpenAPI operation being modified.</param>
        /// <param name="context">The operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            // Ensure the parameter is added only for controller actions
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "TenantIdentify",
                    In = ParameterLocation.Header,
                    Description = "Tenant Id",
                    Required = false
                });
            }
        }
    }
}