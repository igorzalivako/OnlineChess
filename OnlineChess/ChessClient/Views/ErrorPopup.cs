using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace ChessClient.Views;
public class ErrorPopup : Popup
{
    public ErrorPopup(string message, string title, string image, int width, int height)
    {
        this.CanBeDismissedByTappingOutsideOfPopup = true;
        this.Color = Colors.Transparent; // Прозрачный фон всего попапа

        // Основной контейнер
        var border = new Border
        {
            BackgroundColor = Color.FromArgb("#FEF2F2"), // Светло-красный фон
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20, 20, 20, 20)
            },
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.2f,
                Radius = 15,
                Offset = new Point(0, 5)
            },
            Padding = new Thickness(25, 20),
            Content = new VerticalStackLayout
            {
                Spacing = 15,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    // Иконка ошибки
                    new Image
                    {
                        Source = image, // Добавьте файл в Resources/Images
                    },
                    
                    // Заголовок
                    new Label
                    {
                        Text = title,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#DC2626"), // Красный цвет
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                    },
                    
                    // Сообщение
                    new Label
                    {
                        Text = message,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#475569"), // Серый цвет
                        HorizontalOptions = LayoutOptions.Center,
                        MaxLines = 3,
                        LineBreakMode = LineBreakMode.WordWrap,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    
                    // Кнопка
                    new Button
                    {
                        VerticalOptions = LayoutOptions.End,
                        Text = "Понятно",
                        CornerRadius = 12,
                        BackgroundColor = Color.FromArgb("#DC2626"),
                        TextColor = Colors.White,
                        FontSize = 16,
                        Padding = new Thickness(25, 12),
                        Command = new Command(() => Close())
                    }
                }
            }
        };

        this.Content = border;

        // Анимация появления
        this.Opened += OnPopupOpened;

        this.Size = new Size(width, height);
    }

    private async void OnPopupOpened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        // Начальное состояние для анимации
        this.Content.Scale = 0.8;
        this.Content.Opacity = 0;

        // Плавное появление
        await this.Content.FadeTo(1, 200);
        await this.Content.ScaleTo(1, 150, Easing.SinOut);
    }
}