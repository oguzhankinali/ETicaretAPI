using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Infrastructure.Services.Storage;
using ETicaretAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
        }

        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : class, IStorage
        {
            serviceCollection.AddScoped<IStorage, T>();
            serviceCollection.AddScoped<IStorageService, StorageService>();
        }
    }
}