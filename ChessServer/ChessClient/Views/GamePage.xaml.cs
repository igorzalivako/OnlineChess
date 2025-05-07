using ChessClient.ViewModels;
using ChessClient.Utilities.Converters;

namespace ChessClient.Views;

public partial class GamePage : ContentPage
{
	public GamePage(GameViewModel gameViewModel)
	{
		InitializeComponent();
		BindingContext = gameViewModel;
    }
}