using ChessClient.ViewModels;

namespace ChessClient.Views;

public partial class AuthPage : ContentPage
{
	public AuthPage(AuthViewModel authViewModel)
	{
        InitializeComponent();
		BindingContext = authViewModel;
	}
}