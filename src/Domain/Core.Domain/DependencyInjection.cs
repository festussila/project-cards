using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Core.Domain.Enums;
using Core.Domain.Infrastructure.Database;
using Core.Domain.Infrastructure.Services;

namespace Core.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CardContext>(options =>
            options.EnableSensitiveDataLogging(true).UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CardContext).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__CardMigrationsHistory", nameof(Schemas.task));                    
            }));

        services.AddTransient<IDateTimeService, DateTimeService>();

        return services;
    }
}