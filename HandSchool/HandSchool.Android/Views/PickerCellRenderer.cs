using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PickerCell), typeof(PickerCellRenderer))]
namespace HandSchool.Droid
{
    class PickerCellRenderer : CellRenderer
    {
        Android.Widget.TextView cell;
        public static readonly BindableProperty ReusableCellProperty =
        BindableProperty.Create(
        propertyName: "ReusableCell",
        returnType: typeof(Android.Views.View),
        declaringType: typeof(PickerCell),
        defaultValue: default(Android.Views.View)
    );

        const string CellName = "HandSchool.PickerCell";
        public  new  Android.Views.View GetCell(Cell item, Android.Views.View reusableCell, ViewGroup parent, Context context)
        {
            if (!(reusableCell is Android.Views.View tvc))
                tvc = new TextView(context);
            var pc = item as PickerCell;
            pc.SetValue(ReusableCellProperty, tvc);
            //pc.Tapped += ShowTap;
            //pc.PropertyChanged += HandlePropertyChanged;
            (tvc as TextView).Text = pc.Title;
            //tvc.DetailTextLabel.Text = pc.Items[pc.SelectedIndex];
            //tvc.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            return tvc;
        }
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            var nativeCell = (PickerCell)item;
            //Console.WriteLine("\t\t" + nativeCell.Name);

            cell = convertView as Android.Widget.TextView;
            if (cell == null)
            {
                cell = new Android.Widget.TextView(context);
                cell.Text = nativeCell.Title + ":" + nativeCell.Items[nativeCell.SelectedIndex];
                MyListener myListener = new MyListener();
                myListener.Items=nativeCell.Items.ToList();
                cell.SetOnClickListener(myListener);
                
                    
            }
            else
            {
                //cell.NativeCell.PropertyChanged -= OnNativeCellPropertyChanged;
            }

            //nativeCell.PropertyChanged += OnNativeCellPropertyChanged;

            //cell.UpdateCell(nativeCell);
            return cell;
        }
    }
    class MyListener : Java.Lang.Object ,Android.Views.View.IOnClickListener
    {
        public List<string> Items=new List<string>();
        public int SelectedIndex = 0;
        public event EventHandler<DialogClickEventArgs> Handler;
        public void OnClick(Android.Views.View v)
        {
            if(Handler==null)
            {
                Handler = (sender, e) => { SelectedIndex = e.Which; };
            }
            AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.ActivityContext);
            builder.SetSingleChoiceItems(Items.ToArray(), SelectedIndex, Handler);
            builder.Show();
        }
    }

}