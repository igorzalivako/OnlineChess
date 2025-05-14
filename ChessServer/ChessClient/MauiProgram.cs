using Microsoft.Extensions.Logging;
using ChessClient.ViewModels;
using ChessClient.Services;
using ChessClient.Views;
using CommunityToolkit.Maui;
using ChessClient.Models;

namespace ChessClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Добавьте эту строку
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<AuthViewModel>();
            builder.Services.AddTransient<AuthPage>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<GameViewModel>();
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddSingleton<ChessBoardModel>();

            //builder.Services.AddSingleton<GameService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
