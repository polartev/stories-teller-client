using Story_Teller.ServiceCollections;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace Story_Teller;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddUIServices();

        builder.Services.AddSingleton<IServices.IWebSocketService, Services.WebSocketService>();
        builder.Services.AddSingleton<IServices.IConnectionService, Services.ConnectionService>();

        builder.Services.AddSingleton<ViewModels.MainViewModel>();
        builder.Services.AddSingleton<Views.MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
