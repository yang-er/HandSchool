using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Models
{
    public class CurriculumLabel : StackLayout
    {
        public CurriculumItem Context { get; }
        public int ColorId { get; private set; }
        public Span Title = new Span { FontAttributes = FontAttributes.Bold, ForegroundColor = Color.White };
        public Span At = new Span { Text = "\n" };
        public Span Where = new Span { ForegroundColor = Color.FromRgba(255, 255, 255, 220) };

        public CurriculumLabel(CurriculumItem value, int id)
        {
            Context = value;
            ColorId = id;
            Padding = new Thickness(5);
            Children.Add(new Label {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FormattedText = new FormattedString { Spans = { Title, At, Where } },
                VerticalOptions = HorizontalOptions = LayoutOptions.CenterAndExpand
            });
            Update();
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await ItemTapped()),
                NumberOfTapsRequired = 2
            });
        }

        public void Update()
        {
            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, Context.DayEnd - Context.DayBegin + 1);
            Title.Text = Context.Name;
            Where.Text = Context.Classroom;
            BackgroundColor = GetColor();
        }

        public void Unregister()
        {

        }

        private async Task ItemTapped()
        {
            var page = new CurriculumPage(this);
            await page.ShowAsync(Navigation);
        }

        public Color GetColor()
        {
            // thanks to brady
            return Color.FromHex(Internal.Helper.ScheduleColors[ColorId % 8]);
        }
    }
}
