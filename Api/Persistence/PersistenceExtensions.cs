using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace SafeNote.Api.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddEfAndDataProtection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        ).AddDataProtection().PersistKeysToDbContext<AppDbContext>();

        return services;
    }
}