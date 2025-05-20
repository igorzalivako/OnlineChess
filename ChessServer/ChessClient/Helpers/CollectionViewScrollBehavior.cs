using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;

namespace ChessClient.Helpers;

public static class CollectionViewScrollBehavior
{
    public static readonly BindableProperty IsScrollDisabledProperty =
        BindableProperty.CreateAttached(
            "IsScrollDisabled",
            typeof(bool),
            typeof(CollectionViewScrollBehavior),
            false
        );

    public static bool GetIsScrollDisabled(BindableObject view) =>
        (bool)view.GetValue(IsScrollDisabledProperty);

    public static void SetIsScrollDisabled(BindableObject view, bool value) =>
        view.SetValue(IsScrollDisabledProperty, value);
}