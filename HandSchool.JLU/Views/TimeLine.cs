using HandSchool.Internal;
using HandSchool.JLU.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class TimeLine : TouchableFrame
    {
        private BoxView _line;

        public double Weight { get; set; }
        public Time Start { get; set; }
        public Time End { get; set; }

        public void SetBindingMsg(Time start,Time end,object bindingContext)
        {
            Start = start;
            End = end;
            BindingContext = bindingContext;
        }
        public enum TimeLineState
        {
            Start = -1,Mid = 0,End = 1, StartAndEnd = 2
        }
        private TimeLine(double weight, Color color,TimeLineState state)
        {
            Weight = weight;
            HeightRequest = 0;
            Margin = Padding = new Thickness(0);
            CornerRadius = 0;
            WidthRequest = 3;
            Content = _line = new BoxView {Color = color};
            HasShadow = false;
            _line.CornerRadius = state switch
            {
                TimeLineState.Start => new CornerRadius(10, 10, 0, 0),
                TimeLineState.Mid => 0,
                TimeLineState.End => new CornerRadius(0, 0, 10, 10),
                TimeLineState.StartAndEnd => new CornerRadius(10, 10, 10, 10),
                _ => _line.CornerRadius
            };
        }
        public static TimeLine GetFree(int weight, TimeLineState state = TimeLineState.Mid)
        {
            var res = new TimeLine(weight,Color.FromHex("#E8D2AA"),state);
            return res;
        }
        public static TimeLine GetUsing(int weight,TimeLineState state = TimeLineState.Mid)
        {
            var res = new TimeLine(weight,Color.FromHex("#4682b4"), state);
            return res;
        }
        public static TimeLine GetOutTime(int weight,TimeLineState state = TimeLineState.Mid)
        {
            var res = new TimeLine(weight,Color.FromHex("#BEBEBE"),state);
            return res;
        }
        public static TimeLine GetClosed(int weight, TimeLineState state = TimeLineState.Mid)
        {
            var res = new TimeLine(weight, Color.Black, state);
            return res;
        }
        public string TextMessage { get; set; }
    }
}