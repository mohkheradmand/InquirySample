using InquirySample.Application.Common.Interfaces;
using InquirySample.Infrastruture.Persistance;
using InquirySample.Infrastruture.Services.Provider;
using InquirySample.Infrastruture.Services.Providerl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquirySample.Infrastruture;

public static class ConfigureService
{
    public static IServiceCollection InfrastractureServiceProvider(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("InquirySample");

            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        }, ServiceLifetime.Scoped);

        services.AddScoped<IReadDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<IProviderService, ProviderService>();
        services.AddHostedService<ProviderServiceInitializer>();


        return services;
    }
}
