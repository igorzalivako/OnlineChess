using ChessClient.Services;
using ChessClient.ViewModels;
using System.Diagnostics;

namespace ChessClient
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnFrameTapped(object sender, EventArgs e)
        {
            if (dropdownMenu.IsVisible)
            {
                // Анимация закрытия
                await Task.WhenAll(
                    dropdownMenu.FadeTo(0, 150),
                    dropdownMenu.ScaleTo(0.8, 150)
                );
                dropdownMenu.IsVisible = false;
            }
            else
            {
                // Анимация открытия
                dropdownMenu.IsVisible = true;
                await Task.WhenAll(
                    dropdownMenu.FadeTo(1, 200),
                    dropdownMenu.ScaleTo(1, 200)
                );
            }
        }
    }
}
