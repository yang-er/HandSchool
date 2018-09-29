using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulePage : PopContentPage
	{
        public double FontSize => 14;
        private RowDefinition DefRow;
        private ColumnDefinition DefCol;
        private GridLength RowHeight, ColWidth;
        public int Week = -1;

        private bool IsWider = false, forceSize = true;

        public SchedulePage()
		{
			InitializeComponent();
            ShowIsBusyDialog = true;

            RowHeight = new GridLength(60, GridUnitType.Absolute);
            ColWidth = new GridLength(100, GridUnitType.Absolute);
            DefCol = new ColumnDefinition { Width = ColWidth };
            DefRow = new RowDefinition { Height = GridLength.Star };

            ViewModel = ScheduleViewModel.Instance;
            ScheduleViewModel.Instance.RefreshComplete += LoadList;

            foreach (var view in grid.Children)
                (view as Label).FontSize = FontSize;
            
            for (int ij = 1; ij <= 7; ij++)
            {
                grid.ColumnDefinitions.Add(DefCol);
            }

            for (int ij = 1; ij <= Core.App.DailyClassCount; ij++)
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

            SizeChanged += SetTileSize;
            IsWider = false;
            forceSize = true;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadList();
            Core.Log("SchedulePage.OnAppearing. Redrawing.");
        }

        private async void ShowActionList(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("课程表", "取消", null, "刷新课表", "添加课程", "没有地点的课", "显示所有周");

            switch (result)
            {
                case "刷新课表":
                    ScheduleViewModel.Instance.RefreshCommand.Execute(null);
                    break;
                case "添加课程":
                    ScheduleViewModel.Instance.AddCommand.Execute(Navigation);
                    break;
                case "没有地点的课":
                    break;
                case "显示所有周":
                    break;
                default:
                    break;
            }
        }

        public void LoadList()
        {
            for (int i = grid.Children.Count; i > 7 + Core.App.DailyClassCount; i--)
            {
                grid.Children.RemoveAt(i - 1);
            }

            // Render classes
            Core.App.Schedule.RenderWeek(ScheduleViewModel.Instance.Week, out var list);
            for (int i = 0; i < list.Count; i++)
                grid.Children.Add(new CurriculumLabel(list[i], i));
        }

        void SetTileSize(object sender, EventArgs e)
        {
            if (Width > Height && (!IsWider || forceSize))
            {
                forceSize = false;
                IsWider = true;
                DefCol.Width = GridLength.Star;
                DefRow.Height = RowHeight;
                scroller.Orientation = ScrollOrientation.Vertical;
#if __IOS__
                scroller.Margin = new Thickness(0, 0, 0, 0);
#endif
            }
            else if (Width < Height && (IsWider || forceSize))
            {
                forceSize = false;
                IsWider = false;
                DefRow.Height = GridLength.Star;
                DefCol.Width = ColWidth;
                scroller.Orientation = ScrollOrientation.Horizontal;
#if __IOS__
                scroller.Margin = new Thickness(0, iOS.NavigationPageRenderer.NavigationBarHeight, 0, 0);
#endif
            }
        }
    }
}