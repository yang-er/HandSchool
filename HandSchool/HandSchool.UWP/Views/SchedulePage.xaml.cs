using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HandSchool.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SchedulePage : ViewPage
    {
        public int TileFontSize => 14;

        public SchedulePage()
        {
            InitializeComponent();

            BindingContext = ScheduleViewModel.Instance;

            var Brush = new SolidColorBrush(Colors.Gray);
            for (int ij = 1; ij <= Core.App.DailyClassCount; ij++)
            {
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                var label = new TextBlock { Text = ij.ToString(), FontSize = TileFontSize, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brush };
                Grid.SetRow(label, ij);
                Grid.Children.Add(label);
            }

        }
    }
}
