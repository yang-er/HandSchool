using Android.Content;
using Android.Views;
using Android.Widget;
using HandSchool.Internals;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ACell = HandSchool.Views.TextCell;
using ARender = HandSchool.Droid.Renderers.TextCellRenderer;
using AView = Android.Views.View;

[assembly: ExportCell(typeof(ACell), typeof(ARender))]
namespace HandSchool.Droid.Renderers
{
    public class TextCellRenderer : ViewCellRenderer
    {
        protected override AView GetCellCore(Cell item,
            AView convertView, ViewGroup parent, Context context)
        {
            if (item is ACell cell && cell.PreferedCardView == 1)
            {
                return new TextCellView(context, Resource.Layout.cell_text1, cell);
            }
            else
            {
                return base.GetCellCore(item, convertView, parent, context);
            }
        }
    }

    public class TextCellView : LinearLayout, INativeElementView
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
        
        public TextCellView(Context context, int layout, ACell cell) : base(context)
        {
            BindingContext = cell;
            BindingContext.PropertyChanged += CellChanged;
            var layoutInflater = LayoutInflater.FromContext(context);
            var inner = layoutInflater.Inflate(layout, this, true);
            
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.Has<BindViewAttribute>())
                {
                    var attr = prop.Get<BindViewAttribute>();
                    prop.SetValue(this, inner.FindViewById(attr.ResourceId));
                }
            }
            
            Title.Text = BindingContext.Title;
            Detail.Text = BindingContext.Detail;
            RightUp.Text = BindingContext.RightUp;
            RightDown.Text = BindingContext.RightDown;
        }
    }
}