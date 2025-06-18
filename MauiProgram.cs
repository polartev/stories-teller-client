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

        builder.Services.AddSingleton<IServices.IUserService, Services.UserService>();
        builder.Services.AddSingleton<IServices.ILanguageService, Services.LanguageService>();
        builder.Services.AddSingleton<IServices.IWebSocketService, Services.WebSocketService>();
        builder.Services.AddSingleton<IServices.IHttpsService, Services.HttpsService>();
        builder.Services.AddSingleton<IServices.IConnectionService, Services.ConnectionService>();
        builder.Services.AddSingleton<IServices.IImageStorageService, Services.ImageStorageService>();
        builder.Services.AddSingleton<IServices.IStoryStorageService, Services.StoryStorageService>();

        builder.Services.AddSingleton<ViewModels.FlyoutFooterViewModel>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<Models.User>();
        builder.Services.AddSingleton<ViewModels.UserViewModel>();

        builder.Services.AddSingleton<ViewModels.BookshelfViewModel>();
        builder.Services.AddSingleton<Views.BookshelfPage>();

        builder.Services.AddTransient<ViewModels.EditorViewModel>();
        builder.Services.AddTransient<Views.EditorPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
