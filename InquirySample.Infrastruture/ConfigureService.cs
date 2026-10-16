using InquirySample.Application.Common.Interfaces;
using InquirySample.Infrastructure.Services;
using InquirySample.Infrastruture.Persistance;
using InquirySample.Infrastruture.Services.Inquiry;
using InquirySample.Infrastruture.Services.Provider;
using InquirySample.Infrastruture.Services.Providerl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquirySample.Infrastruture;

public static class ConfigureService
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("InquirySample");

            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        }, ServiceLifetime.Scoped);

        services.AddScoped<IReadDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IWriteDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddStackExchangeRedisCache(cfg =>
        {
            cfg.Configuration = configuration.GetConnectionString("Redis");
            cfg.InstanceName = "InquirySample:";
        });

        services.AddHttpClient();

        services.AddSingleton<HsCodeLockProvider>();
        services.AddSingleton<IProviderService, ProviderService>();
        services.AddHostedService<ProviderServiceInitializer>();
        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<IInquiryExecutionService, InquiryExecutionService>();
        services.AddScoped<IProviderHttpClientService, ProviderHttpClientService>();
        return services;
    }
}
