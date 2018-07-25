using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP.Views
{
    public sealed partial class CardView : UserControl
    {
        public Symbol Icon { get; set; } = Symbol.Emoji;
        public UIElementCollection Children => StackPanel.Children;

        public CardView()
        {
            InitializeComponent();
        }
    }
}
