using Android.Content;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(InputCell), typeof(InputCellRenderer))]
namespace HandSchool.Droid
{
    public class InputCellRenderer : EntryCellRenderer
    {
        protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
        {
            var cell = base.GetCellCore(item, convertView, parent, context);
            
            if ((Cell as InputCell).IsPassword && (cell as EntryCellView)?.EditText is TextView textField && textField.TransformationMethod != PasswordTransformationMethod.Instance)
            {
                textField.TransformationMethod = PasswordTransformationMethod.Instance;
            }
            return cell;
        }
    }
}