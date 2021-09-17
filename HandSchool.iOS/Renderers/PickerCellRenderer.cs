using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using HandSchool.Views;
using HandSchool.iOS;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(PickerCell), typeof(PickerCellRenderer))]
namespace HandSchool.iOS
{
    public class PickerCellRenderer : CellRenderer
    {
 
        public static readonly BindableProperty ReusableCellProperty =
            BindableProperty.Create(
                propertyName: "ReusableCell",
                returnType: typeof(UITableViewCell),
                declaringType: typeof(PickerCell),
                defaultValue: default(UITableViewCell)
            );

        const string CellName = "HandSchool.PickerCell";
        
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            if (!(reusableCell is CellTableViewCell tvc))
                tvc = new CellTableViewCell(UITableViewCellStyle.Value1, CellName);
            var pc = item as PickerCell;
            pc.SetValue(ReusableCellProperty, tvc);
            pc.Tapped += ShowTap;
            pc.PropertyChanged += HandlePropertyChanged;
            tvc.TextLabel.Text = pc.Title;
            tvc.DetailTextLabel.Text = pc.Items[pc.SelectedIndex];
            tvc.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            return tvc;
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pickCell = (PickerCell)sender;
            var realCell = (CellTableViewCell)pickCell.GetValue(ReusableCellProperty);

            if (e.PropertyName == PickerCell.SelectedIndexProperty.PropertyName)
                realCell.DetailTextLabel.Text = pickCell.Items[pickCell.SelectedIndex];
            else if (e.PropertyName == PickerCell.TitleProperty.PropertyName)
                realCell.TextLabel.Text = pickCell.Title;
        }

        private async void ShowTap(object sender, EventArgs args)
        {
            var pickCell = (PickerCell)sender;
            if (pickCell.Father == null) return;
            var ops = new string[pickCell.Items.Count];
            for(var i = 0; i < ops.Length; i++)
            {
                ops[i] = pickCell.Items[i];
            }

            var res = await pickCell.Father.RequestActionAsync(pickCell.Title, "取消", null, ops);
            
            for (var i = 0; i < ops.Length; i++)
            {
                if(ops[i] == res)
                {
                    pickCell.SelectedIndex = i;
                    return;
                }
                ops[i] = pickCell.Items[i];
            }
        }
    }
}