using ChessClient.Services;
using ChessClient.ViewModels;
using ChessClient.Views;
using Microsoft.Extensions.Logging;
using Refit;

namespace ChessClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Конфигурация Refit
            builder.Services.AddRefitClient<IChessApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new System.Uri("https://localhost.com");
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                });

            // Регистрация сервисов
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<GameHubService>();
            builder.Services.AddSingleton(Connectivity.Current);

            // Регистрация ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<GameViewModel>();

            // Регистрация страниц
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<GamePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}