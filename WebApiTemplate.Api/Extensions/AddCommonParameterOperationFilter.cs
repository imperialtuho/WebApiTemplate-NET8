using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiTemplate.Api.Extensions
{
    /// <summary>
    /// Swagger operation filter.
    /// </summary>
    public class AddCommonParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

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