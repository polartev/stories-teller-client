using Story_Teller.IServices;
using Story_Teller.Services;

namespace Story_Teller.ServiceCollections;

public static class UIServiceCollectionExtensions
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddSingleton<IAlertService, AlertService>();

        return services;
    }
}
