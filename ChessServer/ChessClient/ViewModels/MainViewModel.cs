using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ChessClient.ViewModels;
public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task ShowLogin()
    {
        await Shell.Current.GoToAsync("//auth?mode=login");
    }

    [RelayCommand]
    private async Task ShowRegister()
    {
        await Shell.Current.GoToAsync("//auth?mode=register");
    }
}