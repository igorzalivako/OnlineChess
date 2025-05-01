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
    }
}
