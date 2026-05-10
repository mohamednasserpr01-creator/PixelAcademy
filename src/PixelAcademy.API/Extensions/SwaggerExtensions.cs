using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Application.DTOs.Users;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PixelAcademy.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithVersioning(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });

            // Schema Filters for Examples
            options.MapType<RegisterRequestDto>(() => new OpenApiSchema
            {
                Type = "object",
                Example = new OpenApiObject
                {
                    ["phoneNumber"] = new OpenApiString("01012345678"),
                    ["username"] = new OpenApiString("johndoe"),
                    ["password"] = new OpenApiString("StrongPassword123!"),
                    ["fullName"] = new OpenApiString("John Ahmed Doe"),
                    ["parentPhoneNumber"] = new OpenApiString("01098765432"),
                    ["governorate"] = new OpenApiString("Cairo"),
                    ["address"] = new OpenApiString("123 Nile Street, Zamalek"),
                    ["schoolName"] = new OpenApiString("Cairo International School"),
                    ["educationalStageId"] = new OpenApiString("00000000-0000-0000-0000-000000000001"),
                    ["educationStreamId"] = new OpenApiString("00000000-0000-0000-0000-000000000010")
                }
            });

            options.MapType<LoginRequestDto>(() => new OpenApiSchema
            {
                Type = "object",
                Example = new OpenApiObject
                {
                    ["phoneNumber"] = new OpenApiString("01012345678"),
                    ["password"] = new OpenApiString("Student123!")
                }
            });

            options.MapType<CreateCourseRequestDto>(() => new OpenApiSchema
            {
                Type = "object",
                Example = new OpenApiObject
                {
                    ["title"] = new OpenApiString("Advanced C# Programming"),
                    ["description"] = new OpenApiString("Master advanced C# concepts and patterns."),
                    ["category"] = new OpenApiString("Programming"),
                    ["level"] = new OpenApiInteger(2),
                    ["price"] = new OpenApiDouble(49.99)
                }
            });

            options.MapType<UpdateProfileRequestDto>(() => new OpenApiSchema
            {
                Type = "object",
                Example = new OpenApiObject
                {
                    ["firstName"] = new OpenApiString("Updated"),
                    ["lastName"] = new OpenApiString("Name"),
                    ["bio"] = new OpenApiString("Passionate developer."),
                    ["phoneNumber"] = new OpenApiString("+1234567890"),
                    ["dateOfBirth"] = new OpenApiString("1990-01-01")
                }
            });

            options.DocumentFilter<OrderByControllerNameDocumentFilter>();
        });

        return services;
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        if (operation.Parameters == null) return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
            if (description == null) continue;

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
            }

            parameter.Required |= description.IsRequired;
        }
    }
}

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(Asp.Versioning.ApiExplorer.ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "PixelAcademy API",
            Version = description.ApiVersion.ToString(),
            Description = "Production-ready e-learning platform API built with .NET 8 Clean Architecture.",
            Contact = new OpenApiContact
            {
                Name = "PixelAcademy Support",
                Email = "support@pixelacademy.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

public class OrderByControllerNameDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.OrderBy(p => p.Key).ToList();
        var orderedPaths = new OpenApiPaths();
        foreach (var path in paths)
        {
            orderedPaths.Add(path.Key, path.Value);
        }
        swaggerDoc.Paths = orderedPaths;
    }
}
