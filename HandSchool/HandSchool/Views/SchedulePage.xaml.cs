using System;
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
        public int Week = -1;

        private bool IsWider = false, IsBusyLoading = false;

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

            RefreshButton.Command = new Command(async () => {
                if (IsBusyLoading) return;
                IsBusyLoading = true;
                var alert = Internal.Helper.ShowLoadingAlert("正在加载课程表……");
                await App.Current.Schedule.Execute();
                LoadList();
                alert.Invoke();
                IsBusyLoading = false;
            });

            AddButton.Command = new Command(async () => await (new CurriculumPage(new CurriculumItem { IsCustom = true, CourseID = "CUSTOM-" + DateTime.Now.ToString() }, true)).ShowAsync(Navigation));
            SizeChanged += SetTileSize;

            IsWider = false;
            DefRow.Height = GridLength.Star;
            DefCol.Width = ColWidth;
            scroller.Orientation = ScrollOrientation.Horizontal;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Week == -1) Week = App.Current.Service.CurrentWeek;
            CurrentWeekShow.Text = $"第{Week}周";
            LoadList();
            System.Diagnostics.Debug.WriteLine("SchedulePage.OnAppearing. Redrawing.");
        }

        void LoadList()
        {
            int i = 7 + App.Current.DailyClassCount;
            while (grid.Children.Count > i)
            {
                grid.Children.RemoveAt(i);
            }
            
            // Render classes
            App.Current.Schedule.RenderWeek(Week, grid.Children);
        }

        void SetTileSize(object sender, EventArgs e)
        {
            if (Width > Height && !IsWider) 
            {
                IsWider = true;
                DefCol.Width = GridLength.Star;
                DefRow.Height = RowHeight;
                scroller.Orientation = ScrollOrientation.Vertical;
            }
            else if (Width < Height && IsWider)
            {
                IsWider = false;
                DefRow.Height = GridLength.Star;
                DefCol.Width = ColWidth;
                scroller.Orientation = ScrollOrientation.Horizontal;
            }
        }
    }
}