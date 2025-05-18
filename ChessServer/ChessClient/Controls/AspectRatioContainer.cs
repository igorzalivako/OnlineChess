using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Controls
{
    using Microsoft.Maui.Controls;

    namespace ChessClient.Controls
    {
        public class AspectRatioContainer : ContentView
        {
            public static readonly BindableProperty AspectRatioProperty =
                BindableProperty.Create(nameof(AspectRatio), typeof(double), typeof(AspectRatioContainer), 1.0);

            public double AspectRatio
            {
                get => (double)GetValue(AspectRatioProperty);
                set => SetValue(AspectRatioProperty, value);
            }

            protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
            {
                double ratio = AspectRatio;
                double width = widthConstraint;
                double height = heightConstraint;

                if (width / ratio > height)
                    width = height * ratio;
                else
                    height = width / ratio;

                return base.OnMeasure(width, height);
            }

            protected override void OnSizeAllocated(double width, double height)
            {
                base.OnSizeAllocated(width, height);

                double ratio = AspectRatio;
                double newWidth = width;
                double newHeight = height;

                if (width / ratio > height)
                    newWidth = height * ratio;
                else
                    newHeight = width / ratio;

                this.Content.WidthRequest = newWidth;
                this.Content.HeightRequest = newHeight;
            }
        }
    }
}
