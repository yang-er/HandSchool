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
        public double FontSize => Device.GetNamedSize(NamedSize.Micro, typeof(Label));

        public SchedulePage()
		{
			InitializeComponent();
            BindingContext = this;
            foreach (var view in grid.Children)
                (view as Label).FontSize = FontSize;
            
            var everyClass = new RowDefinition { Height = new GridLength(1.0 / App.DailyClassCount, GridUnitType.Star) };
            for (int ij = 1; ij <= App.DailyClassCount; ij++)
            {
                grid.RowDefinitions.Add(everyClass);
                grid.Children.Add(new Label()
                {
                    Text = ij.ToString(),
                    FontSize = FontSize,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Gray
                }, 0, ij);
            }

            RefreshButton.Command = new Command(() => { App.Schedule.Execute(); LoadList(); });

            LoadList();
        }

        void LoadList()
        {
            int i = 7 + App.DailyClassCount;
            while (grid.Children.Count > i)
            {
                grid.Children.RemoveAt(i);
            }
            
            // Render classes
            Internal.CurriculumSchedule.RenderWeek(2);
            foreach (var tc in Internal.CurriculumSchedule.List)
            {
                var item = new Label()
                {
                    Text = tc.Source.Name + " @ " + tc.Source.Classroom,
                    FontSize = FontSize,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = Color.LimeGreen
                };
                grid.Children.Add(item, tc.Source.WeekDay, tc.Source.DayBegin);
                Grid.SetRowSpan(item, tc.Source.DayEnd - tc.Source.DayBegin + 1);
            }
        }
    }
}