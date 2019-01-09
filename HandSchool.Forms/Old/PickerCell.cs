using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.Views
{
    public class PickerCell : Cell
    {
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(
                propertyName: nameof(SelectedIndex),
                returnType: typeof(int),
                declaringType: typeof(PickerCell),
                defaultValue: default(int),
                defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(PickerCell),
                defaultValue: default(string)
            );

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public IList<string> Items { get; } = new LockableObservableListWrapper();
    }
}