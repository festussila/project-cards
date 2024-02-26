using System.Text;
using System.Text.Json;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;

using Core.Domain.Enums;
using Core.Domain.Entities;
using Cards.WebAPI.Models.Common;
using Cards.WebAPI.Models.Filters;
using Cards.WebAPI.Models.Swagger;
using Cards.WebAPI.Models.Configuration;
using Core.Domain.Infrastructure.Database;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Models.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequiredLength = MinPassword;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1d);
            options.Lockout.MaxFailedAccessAttempts = 5;
        }).AddEntityFrameworkStores<CardContext>().AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        //Add token validation parameters
        TokenValidationParameters tokenParameters = new()
        {
            //what to validate
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration[JwtIssuer],
            ValidAudience = configuration[JwtAudience],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[JwtKey]!)),
            ClockSkew = new TimeSpan(0)
        };

        //Add JWT Bearer Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenParameters;
        });

        services.AddAuthorizationBuilder()
                .AddPolicy(nameof(AuthPolicy.GlobalRights), policy => policy.RequireRole(nameof(Roles.Admin), nameof(Roles.Member)));

        return services;
    }

    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ModelStateFilter));
            options.Filters.Add(typeof(ExceptionFilter));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new EmptyStringToNullConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        return services;
    }

    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // specify the default API Version as 1.0
            options.DefaultApiVersion = new ApiVersion(1, 0);

            // if the client hasn't specified the API version in the request, use the default API version number 
            options.AssumeDefaultVersionWhenUnspecified = true;

            // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;

            options.ApiVersionReader = new UrlSegmentApiVersionReader();

        }).AddApiExplorer(options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";            
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        
        services.AddSwaggerGen(options =>
        {
            // set the xml comments path for the Swagger JSON and UI
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            //define how the API is secured by defining one or more security schemes.
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter in the value field: <b>Bearer {your JWT token}</b>"
            });

            OpenApiSecurityScheme apiKeyScheme = new()
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [apiKeyScheme] = Array.Empty<string>()
            });           
        });

        return services;
    }

    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.  
        app.UseSwagger();
      
        //Enable middleware to serve swagger - ui(HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(options =>
        {
            IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version       

            foreach (ApiVersionDescription description in descriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
            options.DocExpansion(DocExpansion.List);
        });

        return app;
    }   
}