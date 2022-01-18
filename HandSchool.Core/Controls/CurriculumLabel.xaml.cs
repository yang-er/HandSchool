using HandSchool.Internals;
using HandSchool.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CurriculumLabel : Frame
    {
        public CurriculumItemBase Context { get; }
        public int ColorId { get; private set; }

#if false
        private void Layout_SizeChanged(object sender, EventArgs e)
        {

            foreach (Span Item in (Children[0] as Label).FormattedText.Spans)
                Item.FontSize = (Children[0] as Label).FontSize;
#if __IOS__
                des.FontSize *= 0.8;
#endif
        //每次都重置一次初始大小 因为此方法在横竖屏切换时可能被多次调用
            var Height = this.Height-10;
            var Width = this.Width-10;
            double NowFontSize = (Children[0] as Label).FontSize;

            while (GetTotalHeight(Width, NowFontSize) >Height&&NowFontSize>1)
                NowFontSize--;

            foreach (Span Item in (Children[0] as Label).FormattedText.Spans)
            {
                Item.FontSize =NowFontSize;
#if __IOS__
            des.FontSize =NowFontSize;
#endif
            }
        }

        private double GetTotalHeight(double Width,double NowFontSize)
        {
            double TotalHeight = 0;
            foreach(var i in Children)
            {
                TotalHeight += GetLabelHeight(i as Label, Width, NowFontSize);
            }
            TotalHeight += (Children.Count - 1 )* NowFontSize;
            return TotalHeight;
        }

        private double GetLabelHeight(Label label,double Width,double NowFontSize)
        {
            
            double TotalHeight = 0;
            double Padding = NowFontSize * 0.42;
            double TextHeight = NowFontSize + Padding;
            string Text = label.FormattedText.ToString();
            string[] SplitedText = Text.Split('\n');
            foreach (string Item in SplitedText)
            {
                TotalHeight += ((int)(1 + Item.Length * (TextHeight-Padding) / Width)) * TextHeight;
            }
            return TotalHeight;
        }
#endif
        private int LongestTitle { get; set; } = 11; //最长的标题长度，用于剪裁课程名称
        private int FontSize { get; set; } = 13;//字体字号
        private FormattedString GetDisplay()
        {
            var formattedString = new FormattedString();
            var desc = Context.ToDescription();

            foreach (var item in desc)
            {
                if (formattedString.Spans.Count > 0) //这里已经有课了，是“所有周”的情况
                {
                    LongestTitle = 9;
                    bool narrow = formattedString.Spans.Count / 3.0 > 2 && GetSpace() <= 2;

                    if (formattedString.Spans.Count == 3)
                    {
                        FontSize = 12;
                        var span = formattedString.Spans[0];
                        if (span.Text.Length > LongestTitle + 3)
                        {
                            span.Text = span.Text.Substring(0, LongestTitle) + "...";
                        }
                        foreach (var i in formattedString.Spans)
                        {
                            if (i.Text == "\n\n")
                                i.FontSize = FontSize >> 2;
                            else i.FontSize = FontSize;
                        }
                    }
                    if (narrow)
                    {
                        LongestTitle = 7;
                        FontSize = 11;

                        foreach (var i in formattedString.Spans)
                        {
                            if (i.Text == "\n\n")
                                i.FontSize = FontSize >> 2;
                            else i.FontSize = FontSize;
                        }
                        for (int i = 0; i < formattedString.Spans.Count; i += 4)
                        {
                            var span = formattedString.Spans[i];
                            if (span.Text.Length > LongestTitle + 3)
                            {
                                span.Text = span.Text.Substring(0, LongestTitle) + "...";
                            }
                        }
                    }
                    formattedString.Spans.Add(new Span { Text = "\n\n", FontSize = narrow ? FontSize >> 2 : FontSize });
                }

                var tit = new Span
                {
                    FontSize = FontSize,
                    FontAttributes = FontAttributes.Bold,
                    ForegroundColor = ColorExtend.ColorDelta(GetColor(), 0.45),
                    Text = item.Title.Length <= LongestTitle ? item.Title : item.Title.Substring(0, LongestTitle) + "...",
                };

                var des = new Span
                {
                    FontSize = FontSize,
                    ForegroundColor = ColorExtend.ColorDelta(GetColor(), 0.4),
                    Text = item.IsCustom ? item.Description : ClassInfoSimplifier.Instance.SimplifyName(item.Description)
                };
                if (Core.Platform.RuntimeName == "iOS")
                    des.FontSize *= 0.9;

                formattedString.Spans.Add(tit);
                formattedString.Spans.Add(new Span { Text = "\n\n", FontSize = tit.FontSize / 4, });
                formattedString.Spans.Add(des);
            }

            return formattedString;
        }
        public CurriculumLabel(CurriculumItemBase value, int id)
        {
            InitializeComponent();
            InnerLabel.BindingContext = Context = value;
            ColorId = id;

            InnerLabel.Children.Add(new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FormattedText = GetDisplay(),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            });

            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, GetSpace());
            BackgroundColor = GetColor();

            if (Context is CurriculumItem)
            {
                GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new CommandAction(EditCurriculum),
                    NumberOfTapsRequired = 2,
                });
            }
        }
        int GetSpace()
        {
            return Context.DayEnd - Context.DayBegin + 1;
        }
        private async Task EditCurriculum()
        {
            var page = Core.New<ICurriculumPage>();
            page.SetNavigationArguments(Context as CurriculumItem, false);

            if (await page.IsSuccess())
                ViewModels.ScheduleViewModel.Instance.SendRefreshComplete();
        }

        public Color GetColor()
        {
            return ColorExtend.ColorFromRgb(ScheduleColors[ColorId % 8]);
        }

        static readonly (int r, int g, int b)[] ScheduleColors =
        {
            (255,248,200),
            (237,237,255),
            (253,235,222),
            (255,220,220),
            (245,215,239),
            (157,233,254),
            (237,237,237),
            (210,235,230)
        };
    }
}