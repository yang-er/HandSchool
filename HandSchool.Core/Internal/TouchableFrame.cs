using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public class TouchableFrame : Frame
    {
        public TouchableFrame()
        {
            switch (Device.RuntimePlatform)
            {
                case "iOS":
                    CornerRadius = 20;
                    break;

                default:
                    CornerRadius = 15;
                    break;
            }
        }
        public ICommand ClickedCommand
        {
            get => (ICommand)GetValue(ClickedCommandProperty);
            set
            {
                SetValue(ClickedCommandProperty, value);
                OnPropertyChanged(nameof(ClickedCommand));
            }
        }
        public ICommand LongClickedCommand
        {
            get => (ICommand)GetValue(LongClickedCommandProperty);
            set
            {
                SetValue(LongClickedCommandProperty, value);
                OnPropertyChanged(nameof(LongClickedCommand));
            }
        }
        public static BindableProperty ClickedCommandProperty = BindableProperty.Create(
            propertyName: nameof(ClickedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchableFrame));
        public static BindableProperty LongClickedCommandProperty = BindableProperty.Create(
            propertyName: nameof(LongClickedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchableFrame));
    }
}
