using Xamarin.Forms;

namespace HandSchool.Views
{
    // thanks to Soar1991@cnblogs
    public class InputCell : EntryCell
    {
        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(propertyName: "IsPassword", returnType: typeof(bool), declaringType: typeof(InputCell), defaultValue: false);

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }
    }
}
