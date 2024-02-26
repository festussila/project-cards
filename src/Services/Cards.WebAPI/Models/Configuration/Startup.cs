using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Core.Domain;
using Core.Management;
using Core.Management.Common.Seedwork;
using Core.Domain.Infrastructure.Database;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Models.Configuration;

public class Startup
{
    //Add services to the container      
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddCoreDomain(configuration);
        services.AddCoreManagement();
        services.AddIdentity();
        services.AddCustomControllers();
        services.AddVersioning();
        services.AddCustomSwagger();
        services.AddAuthentication(configuration);
        services.AddAutoMapper(Assembly.GetAssembly(typeof(Program)));
    }

    //Configure the HTTP request pipeline.
    public static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        app.UseCors(CorsPolicy);

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
    }

    //Ensure migrations and seed database
    public static void ConfigureDatabase(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            CardContext context = services.GetRequiredService<CardContext>();

            if (context.Database.IsSqlServer()) { context.Database.Migrate(); }

            ISeed seed = services.GetRequiredService<ISeed>();
            seed.SeedDefaults().Wait();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error while setting application - migrations and seed");
            throw;
        }
    }
}