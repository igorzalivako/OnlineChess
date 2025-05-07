using Microsoft.Extensions.Logging;
using ChessClient.ViewModels;
using ChessClient.Services;
using ChessClient.Views;

namespace ChessClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
#pragma warning disable CA1416 // Проверка совместимости платформы
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });
#pragma warning restore CA1416 // Проверка совместимости платформы
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<AuthViewModel>();
            builder.Services.AddTransient<AuthPage>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<GameViewModel>();
            builder.Services.AddTransient<GamePage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
