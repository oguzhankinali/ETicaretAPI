using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Infrastructure.Services.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            
        }

        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : class, IStorage
        {
            serviceCollection.AddScoped<IStorage, T>();
            serviceCollection.AddScoped<IStorageService, StorageService>();
        }
    }
}