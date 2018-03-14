using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulePage : ContentPage
	{
        public double FontSize => 14;
        private RowDefinition DefRow;
        private ColumnDefinition DefCol;
        private GridLength RowHeight, ColWidth;

        private bool IsWider = false, firstTime = true;

        public SchedulePage()
		{
			InitializeComponent();

            RowHeight = new GridLength(60, GridUnitType.Absolute);
            ColWidth = new GridLength(100, GridUnitType.Absolute);
            DefCol = new ColumnDefinition { Width = ColWidth };
            DefRow = new RowDefinition { Height = RowHeight };

            BindingContext = this;
            foreach (var view in grid.Children)
                (view as Label).FontSize = FontSize;
            
            for (int ij = 1; ij <= 7; ij++)
            {
                grid.ColumnDefinitions.Add(DefCol);
            }

            for (int ij = 1; ij <= App.Current.DailyClassCount; ij++)
            {
                grid.RowDefinitions.Add(DefRow);
                grid.Children.Add(new Label()
                {
                    Text = ij.ToString(),
                    FontSize = FontSize,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Gray
                }, 0, ij);
            }

            RefreshButton.Command = new Command(() => { App.Current.Schedule.Execute(); LoadList(); });
            SizeChanged += SetTileSize;

            LoadList();
        }

        void LoadList()
        {
            int i = 7 + App.Current.DailyClassCount;
            while (grid.Children.Count > i)
            {
                grid.Children.RemoveAt(i);
            }

            // Render classes
            var p = grid.Children as IList<View>;
            App.Current.Schedule.RenderWeek(2, grid.Children);
        }

        void SetTileSize(object sender, EventArgs e)
        {
            if (Width > Height && (!IsWider || firstTime)) 
            {
                IsWider = true;
                DefCol.Width = GridLength.Star;
                DefRow.Height = RowHeight;
                scroller.Orientation = ScrollOrientation.Vertical;
            }
            else if(Width < Height && (IsWider || firstTime))
            {
                IsWider = false;
                DefRow.Height = GridLength.Star;
                DefCol.Width = ColWidth;
                scroller.Orientation = ScrollOrientation.Horizontal;
            }
            firstTime = false;
        }
    }
}