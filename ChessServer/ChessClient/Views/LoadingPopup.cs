using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace ChessClient.Views;

public class LoadingPopup : Popup
{
    public LoadingPopup(int width = 300, int height = 200)
    {
        this.CanBeDismissedByTappingOutsideOfPopup = false;
        this.Color = Colors.Transparent;

        var border = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20)
            },
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.1f,
                Radius = 15,
                Offset = new Point(0, 5)
            },
            Padding = new Thickness(30, 25),
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new ActivityIndicator
                    {
                        Color = Color.FromArgb("#3B82F6"), // Синий цвет
                        IsRunning = true,
                        WidthRequest = 40,
                        HeightRequest = 40
                    },

                    new Label
                    {
                        Text = "Поиск соперника...",
                        FontSize = 18,
                        TextColor = Color.FromArgb("#1E293B"), // Темно-синий
                        HorizontalOptions = LayoutOptions.Center
                    },
                }
            }
        };

        this.Content = border;
        this.Size = new Size(width, height);
    }

    // Метод для обновления статуса
    /*public void UpdateStatus(string newMessage)
    {
        if (this.Content is Border border &&
            border.Content is VerticalStackLayout layout &&
            layout.Children[2] is Label statusLabel)
        {
            statusLabel.Text = newMessage;
        }
    }*/
}