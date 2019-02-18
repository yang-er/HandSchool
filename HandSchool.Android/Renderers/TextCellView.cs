using Android.Content;
using Android.Views;
using Android.Widget;
using System.ComponentModel;
using Xamarin.Forms;
using ACell = HandSchool.Views.TextCell;
using AView = Android.Views.View;
using AListView = Android.Widget.ListView;

namespace HandSchool.Droid
{
    public class TextCellView : LinearLayout, INativeElementView, IBindTarget
    {
        public ACell BindingContext { get; set; }

        [BindView(Resource.Id.cell_title)]
        public TextView Title { get; set; }

        [BindView(Resource.Id.cell_detail)]
        public TextView Detail { get; set; }

        [BindView(Resource.Id.cell_rightup)]
        public TextView RightUp { get; set; }

        [BindView(Resource.Id.cell_rightdown)]
        public TextView RightDown { get; set; }

        [BindView(Resource.Id.cell_rightdownind)]
        public TextView RightDownIndicator { get; set; }

        [BindView(Resource.Id.cell_attach1)]
        public TextView Attach1 { get; set; }

        public AView Inner { get; }

        public Element Element => BindingContext;

        private void CellChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BindingContext.Title):
                    Title?.SetText(BindingContext.Title);
                    break;

                case nameof(BindingContext.Detail):
                    Detail?.SetText(BindingContext.Detail);
                    break;

                case nameof(BindingContext.RightUp):
                    RightUp?.SetText(BindingContext.RightUp);
                    break;

                case nameof(BindingContext.RightDown):
                    RightDown?.SetText(BindingContext.RightDown);
                    break;

                case nameof(BindingContext.Attach1):
                    Attach1?.SetText(BindingContext.Attach1);
                    break;

                case nameof(BindingContext.RightDownShow):
                    // RightDown?.SetVisibility(BindingContext.RightDownShow);
                    break;

                case nameof(BindingContext.RightDownColor):
                    RightDownIndicator?.SetColor(BindingContext.RightDownColor);
                    break;
            }
        }

        public void SolveBindings()
        {
            Title?.SetText(BindingContext.Title);
            Detail?.SetText(BindingContext.Detail);
            RightUp?.SetText(BindingContext.RightUp);
            RightDown?.SetText(BindingContext.RightDown);
            // RightDown?.SetVisibility(BindingContext.RightDownShow);
            Attach1?.SetText(BindingContext.Attach1);
            RightDownIndicator?.SetColor(BindingContext.RightDownColor);
        }

        public TextCellView(Context context, int layout, ACell cell) : base(context)
        {
            BindingContext = cell;
            BindingContext.PropertyChanged += CellChanged;
            var layoutInflater = LayoutInflater.FromContext(context);
            Inner = layoutInflater.Inflate(layout, this, true);
            this.SolveView(Inner);
        }
    }
}