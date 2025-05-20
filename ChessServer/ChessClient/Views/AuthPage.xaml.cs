using ChessClient.ViewModels;

namespace ChessClient.Views;

public partial class AuthPage : ContentPage
{
	public AuthPage(AuthViewModel authViewModel)
	{
        InitializeComponent();
		BindingContext = authViewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("///MainPage");
        return true; 
    }
}