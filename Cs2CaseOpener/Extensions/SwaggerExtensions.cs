using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cs2CaseOpener.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "CS2 Case Opener API", 
                Version = "v1",
                Description = "API for tracking CS2 case openings"
            });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            
            c.OperationFilter<AuthorizationHeaderOperationFilter>();
        });
        
        return services;
    }
    
    private class AuthorizationHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionMetadata = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            
            if (actionMetadata == null) return;
            
            foreach (var parameter in actionMetadata.Parameters)
            {
                var headerAttribute = parameter.BindingInfo?.BindingSource?.Id == "Header" &&
                                     parameter.BindingInfo?.BinderModelName == "Authorization";
                
                if (headerAttribute)
                {
                    var paramToRemove = operation.Parameters.FirstOrDefault(p => 
                        p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase));
                    
                    if (paramToRemove != null)
                    {
                        operation.Parameters.Remove(paramToRemove);
                    }
                    
                    break;
                }
            }
        }
    }
}