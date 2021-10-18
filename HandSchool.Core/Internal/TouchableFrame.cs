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

        private EventHandler<EventArgs> _click;
        public event EventHandler<EventArgs> Click
        {
            add
            {
                _click += value;
                OnPropertyChanged(nameof(HasClick));
            }
            remove
            {
                _click -= value;
                OnPropertyChanged(nameof(HasClick));
            }
        }

        private EventHandler<EventArgs> _longClick;
        public event EventHandler<EventArgs> LongClick
        {
            add
            {
                _longClick += value;
                OnPropertyChanged(nameof(HasLongClick));
            }
            remove
            {
                _longClick -= value;
                OnPropertyChanged(nameof(HasLongClick));
            }
        }
        public ICommand ClickCommand
        {
            get => (ICommand)GetValue(ClickCommandProperty);
            set
            {
                SetValue(ClickCommandProperty, value);
                OnPropertyChanged(nameof(HasClick));
            }
        }
        public ICommand LongClickCommand
        {
            get => (ICommand)GetValue(LongClickCommandProperty);
            set
            {
                SetValue(LongClickCommandProperty, value);
                OnPropertyChanged(nameof(HasLongClick));
            }
        }
        public static readonly BindableProperty ClickCommandProperty = BindableProperty.Create(
            propertyName: nameof(ClickCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchableFrame));
        public static readonly BindableProperty LongClickCommandProperty = BindableProperty.Create(
            propertyName: nameof(LongClickCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchableFrame));

        public bool HasClick => ClickCommand != null || (_click != null && _click.GetInvocationList().Length != 0);
        public bool HasLongClick => LongClickCommand != null || (_longClick != null && _longClick.GetInvocationList().Length != 0);

        public void OnClick(object sender = null, EventArgs args = null)
        {
            ClickCommand?.Execute(null);
            _click?.Invoke(sender??this, args);
        }

        public void OnLongClick(object sender = null, EventArgs args = null)
        {
            LongClickCommand?.Execute(null);
            _longClick?.Invoke(sender??this, args);
        }
    }
}
