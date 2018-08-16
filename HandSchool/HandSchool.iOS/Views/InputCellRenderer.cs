using HandSchool.iOS;
using HandSchool.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(InputCell), typeof(InputCellRenderer))]
namespace HandSchool.iOS
{
    public class InputCellRenderer : EntryCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var entryCell = (InputCell)item;
            var cell = base.GetCell(item, reusableCell, tv);
            if (cell != null)
            {
                var textField = (UITextField)cell.ContentView.Subviews[0];
                textField.SecureTextEntry = entryCell.IsPassword;
            }
            return cell;
        }
    }
}