using Microsoft.Extensions.DependencyInjection;

using IdGen;

using Core.Management.Interfaces;
using Core.Management.Repositories;
using Core.Management.Common.Seedwork;

namespace Core.Management;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreManagement(this IServiceCollection services)
    {
        services.AddSingleton<IIdGenerator<long>>(_ => new IdGenerator(0, new IdGeneratorOptions(idStructure: new IdStructure(45, 2, 16), timeSource: new DefaultTimeSource(new DateTime(2024, 02, 24, 20, 45, 15, DateTimeKind.Utc)))));

        //Repository Initializations
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICardRepository, CardRepository>();

        //Service Initializations
        services.AddScoped<ISeed, Seed>();

        return services;
    }
}