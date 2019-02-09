using Android.Content;
using Android.Views;
using Android.Widget;
using System.ComponentModel;
using Xamarin.Forms;
using ACell = HandSchool.Views.TextCell;

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

        public Element Element => BindingContext;

        private void CellChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BindingContext.Title):
                    Title.Text = BindingContext.Title;
                    break;

                case nameof(BindingContext.Detail):
                    Detail.Text = BindingContext.Detail;
                    break;

                case nameof(BindingContext.RightUp):
                    RightUp.Text = BindingContext.RightUp;
                    break;

                case nameof(BindingContext.RightDown):
                    RightDown.Text = BindingContext.RightDown;
                    break;
            }
        }

        public void SolveBindings()
        {
            Title.Text = BindingContext.Title;
            Detail.Text = BindingContext.Detail;
            RightUp.Text = BindingContext.RightUp;
            RightDown.Text = BindingContext.RightDown;
        }

        public TextCellView(Context context, int layout, ACell cell) : base(context)
        {
            BindingContext = cell;
            BindingContext.PropertyChanged += CellChanged;
            var layoutInflater = LayoutInflater.FromContext(context);
            var inner = layoutInflater.Inflate(layout, this, true);
            this.SolveView(inner);
        }
    }
}