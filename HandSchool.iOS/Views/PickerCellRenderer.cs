using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using HandSchool.Views;
using HandSchool.iOS;
using System;
using System.ComponentModel;

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

        private void ShowTap(object sender, EventArgs args)
        {
            var pickCell = (PickerCell)sender;
            var uiac = UIAlertController.Create(pickCell.Title, null, UIAlertControllerStyle.ActionSheet);

            for (int i = 0; i < pickCell.Items.Count; i++)
            {
                int j = i;
                uiac.AddAction(UIAlertAction.Create(pickCell.Items[i], UIAlertActionStyle.Default, (act) => pickCell.SelectedIndex = j));
            }

            uiac.AddAction(UIAlertAction.Create("取消", UIAlertActionStyle.Cancel, null));
            MainPageRenderer.GlobalViewController.PresentViewController(uiac, true, null);
        }
    }
}