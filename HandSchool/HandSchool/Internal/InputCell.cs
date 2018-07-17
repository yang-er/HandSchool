using Xamarin.Forms;

namespace HandSchool.Views
{
    /// <summary>
    /// 可以输入密码的项目
    /// </summary>
    public class InputCell : EntryCell
    {
        /// <summary>
        /// IsPassword的储存
        /// </summary>
        public static readonly BindableProperty IsPasswordProperty = 
            BindableProperty.Create(
                propertyName: nameof(IsPassword),
                returnType: typeof(bool),
                declaringType: typeof(InputCell),
                defaultValue: false);

        /// <summary>
        /// 是否为密码类型
        /// </summary>
        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }
    }
}
