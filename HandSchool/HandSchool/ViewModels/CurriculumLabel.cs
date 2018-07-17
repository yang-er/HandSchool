using HandSchool.Views;
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
        
        private async Task ItemTapped()
        {
            var page = new CurriculumPage(this.Context);
            await page.ShowAsync(Navigation);
        }

        public Color GetColor()
        {
            // thanks to brady
            return Color.FromHex(ScheduleColors[ColorId % 8]);
        }

        static readonly string[] ScheduleColors = {
            "#59e09e", "#f48fb1", "#ce93d8", "#ff8a65",
            "#9fa8da", "#42a5f5", "#80deea", "#c6de7c" };
    }
}
