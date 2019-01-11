using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;
using HandSchool.Views;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ATextView = Android.Widget.TextView;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(PickerCell), typeof(PickerCellRenderer))]
namespace HandSchool.Droid
{
    class PickerCellRenderer : CellRenderer
    {
        PickerCellView cellView;
        PickerCell nativeCell;
        Context currentContext;

        const string CellName = "HandSchool.PickerCell";
        
        protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
        {
            currentContext = context;
            nativeCell = (PickerCell)item;

            cellView = convertView as PickerCellView;

            if (cellView == null)
            {
                cellView = new PickerCellView(context, item);
                cellView.EditText.Click += OnClicked;
            }
            
            UpdatePicker();
            return cellView;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == PickerCell.TitleProperty.PropertyName)
                UpdatePicker();
            else if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
                UpdatePicker();
        }

        private void AlertDialogHandler(object sender, DialogClickEventArgs args)
        {
            nativeCell.SelectedIndex = args.Which;
            (sender as AlertDialog).Dismiss();
        }

        private void OnClicked(object sender, EventArgs args)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(currentContext);
            builder.SetSingleChoiceItems(nativeCell.Items.ToArray(), nativeCell.SelectedIndex, AlertDialogHandler);
            builder.Show();
        }

        void UpdatePicker()
        {
            cellView.LabelText = nativeCell.Title;
            
            if (nativeCell.SelectedIndex == -1 || nativeCell.Items == null || nativeCell.SelectedIndex >= nativeCell.Items.Count)
                cellView.EditText.Text = null;
            else
                cellView.EditText.Text = nativeCell.Items[nativeCell.SelectedIndex];
        }
        
        sealed class PickerCellView : LinearLayout, AView.IOnFocusChangeListener, INativeElementView
        {
            public const double DefaultMinHeight = 55;

            readonly Cell _cell;
            readonly ATextView _label;
            string _labelTextText;
            
            public PickerCellView(Context context, Cell cell) : base(context)
            {
                _cell = cell;
                SetMinimumWidth((int)context.ToPixels(50));
                SetMinimumHeight((int)context.ToPixels(36));
                Orientation = Orientation.Horizontal;
                
                var padding = (int)context.ToPixels(8);
                SetPadding((int)context.ToPixels(15), padding, padding, padding);

                _label = new ATextView(context);
                Android.Support.V4.Widget.TextViewCompat.SetTextAppearance(_label, Android.Resource.Style.TextAppearanceSmall);
                _label.SetWidth((int)context.ToPixels(80));

                var layoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) { Gravity = GravityFlags.CenterVertical };
                using (layoutParams)
                    AddView(_label, layoutParams);

                EditText = new EditText(context);
                EditText.KeyListener = null;
                EditText.OnFocusChangeListener = this;
                //editText.SetBackgroundDrawable (null);
                layoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { Width = 0, Weight = 1, Gravity = GravityFlags.FillHorizontal | GravityFlags.Center };
                using (layoutParams)
                    AddView(EditText, layoutParams);
            }
            
            public EditText EditText { get; }

            public Action<bool> FocusChanged { get; set; }

            public string LabelText
            {
                get { return _labelTextText; }
                set
                {
                    if (_labelTextText == value)
                        return;

                    _labelTextText = value;
                    _label.Text = value;
                }
            }

            public Element Element => _cell;
            
            void IOnFocusChangeListener.OnFocusChange(AView view, bool hasFocus)
            {
                FocusChanged?.Invoke(hasFocus);
            }
            
            public void SetRenderHeight(double height)
            {
                SetMinimumHeight((int)Context.ToPixels(height == -1 ? DefaultMinHeight : height));
            }
        }
    }
}
