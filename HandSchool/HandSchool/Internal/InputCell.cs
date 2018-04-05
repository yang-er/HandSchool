using Xamarin.Forms;

namespace HandSchool.Views
{
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
