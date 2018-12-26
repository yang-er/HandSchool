using Microcharts;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class ChartDialog : ContentDialog
    {
        public ChartDialog(Chart charts, string title = "")
        {
            InitializeComponent();
            Title = title;
            chart.Chart = charts;
        }
    }
}
