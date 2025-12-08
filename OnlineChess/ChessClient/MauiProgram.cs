using Microsoft.Extensions.Logging;
using ChessClient.ViewModels;
using ChessClient.Services;
using ChessClient.Views;
using CommunityToolkit.Maui;
using ChessClient.Models;
using ChessClient.Helpers; // Подключи свой namespace!
using Microsoft.Maui.Controls.Handlers.Items;
using CommunityToolkit.Maui.Core;


#if ANDROID
using Android.Views;
#endif

namespace ChessClient
{
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
                });

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });
            builder
                .UseMauiApp<App>()
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.AddHandler<CollectionView, CollectionViewHandler>();
#endif
                });
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCore();
#if ANDROID
            // Добавляем маппинг для отключения скролла
            CollectionViewHandler.Mapper.AppendToMapping("DisableScroll", (handler, view) =>
            {
                var isScrollDisabled = CollectionViewScrollBehavior.GetIsScrollDisabled(view);
                if (isScrollDisabled && handler.PlatformView is AndroidX.RecyclerView.Widget.RecyclerView recyclerView)
                {
                    recyclerView.SetOnTouchListener(new NoScrollTouchListener());
                }
                else if (handler.PlatformView is AndroidX.RecyclerView.Widget.RecyclerView recyclerView2)
                {
                    recyclerView2.SetOnTouchListener(null); // вернуть дефолт
                }
            });
#endif

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
#if ANDROID
    // Класс должен быть объявлен здесь, вне метода!
    class NoScrollTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            // true — запрещает любые тапы, включая скролл
            return true;
        }
    }
#endif
}
