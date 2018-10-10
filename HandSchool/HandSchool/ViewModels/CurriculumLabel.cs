using HandSchool.Views;
using System;
using Xamarin.Forms;

namespace HandSchool.Models
{
    public class CurriculumLabel : StackLayout
    {
        public CurriculumItemBase Context { get; }
        public int ColorId { get; private set; }

        private void Layout_SizeChanged(object sender, EventArgs e)
        {
            var Height = this.Height-10;
            var Width = this.Width-10;
            while(GetTotalHeight(Width)>Height)
            {
                foreach (Span Item in (Children[0] as Label).FormattedText.Spans)
                    Item.FontSize -= 1;
            }
            
        }
        private double GetTotalHeight(double Width)
        {
            double TotalHeight = 0;
            foreach(var i in Children)
            {
                TotalHeight += GetLabelHeight(i as Label, Width);
            }
            TotalHeight += (Children.Count - 1 )* (Children[0] as Label).FontSize;
            return TotalHeight;
        }
        private double GetLabelHeight(Label label,double Width)
        {
            
            double TotalHeight = 0;
            double Padding = 6;
            double TextHeight = label.FormattedText.Spans[0].FontSize + 6;
            string Text = label.FormattedText.ToString();
            string[] SplitedText = Text.Split('\n');
            foreach (string Item in SplitedText)
            {
                TotalHeight += ((int)(1 + Item.Length * (TextHeight-Padding) / Width)) * TextHeight;
            }
            return TotalHeight;
        }
        public CurriculumLabel(CurriculumItemBase value, int id)
        {
            Context = value;
            ColorId = id;
            Padding = new Thickness(5);
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            WidthRequest = 1000;

            var formattedString = new FormattedString();
            var desc = value.ToDescription();
            var test = this.Height;
            var width = this.Width;
            this.SizeChanged += Layout_SizeChanged;
            foreach (var item in desc)
            {
                if (formattedString.Spans.Count > 0)
                {
                    formattedString.Spans.Add(new Span { Text = "\n\n"});
                }

                var tit = new Span
                {
                    FontAttributes = FontAttributes.Bold,
                    ForegroundColor = Color.White,
                    Text = item.Title
                };

                var des = new Span
                {
                    ForegroundColor = Color.FromRgba(255, 255, 255, 220),
                    Text = item.Description
                };

#if __IOS__
                des.FontSize *= 0.8;
#endif
                formattedString.Spans.Add(tit);
                formattedString.Spans.Add(new Span { Text = "\n" });
                formattedString.Spans.Add(des);
            }

            Children.Add(new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FormattedText = formattedString,
                VerticalOptions = HorizontalOptions = LayoutOptions.CenterAndExpand
            });

            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, Context.DayEnd - Context.DayBegin + 1);
            BackgroundColor = GetColor();

            if (Context is CurriculumItem)
            {
                GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(EditCurriculum),
                    NumberOfTapsRequired = 2
                });
            }
        }
        
        private async void EditCurriculum()
        {
            var page = new CurriculumPage(Context as CurriculumItem);
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
