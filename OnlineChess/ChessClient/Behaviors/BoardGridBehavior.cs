using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Behaviors
{
    public class BoardGridBehavior : Behavior<Grid>
    {
        protected override void OnAttachedTo(Grid grid)
        {
            base.OnAttachedTo(grid);
            grid.ParentChanged += OnParentChanged;
        }

        private void OnParentChanged(object sender, EventArgs e)
        {
            if (sender is Grid grid && grid.Parent is View parentView)
            {
                parentView.SizeChanged += (s, args) => UpdateGridSize(grid, parentView);
                UpdateGridSize(grid, parentView);
            }
        }

        private void UpdateGridSize(Grid grid, View parentView)
        {
            var availableHeight = parentView.Height -
                               grid.Margin.VerticalThickness;

            var availableWidth = parentView.Width -
                                grid.Margin.HorizontalThickness;

            var size = Math.Min(availableWidth, availableHeight);
            var maxSize = Math.Min(size, 500); // Максимальный размер доски

            grid.WidthRequest = maxSize;
            grid.HeightRequest = maxSize;
        }
    }
}
