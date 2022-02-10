using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public class TouchableFrame : Frame
    {
        public TouchableFrame()
        {
            CornerRadius = Device.RuntimePlatform switch
            {
                Device.iOS => 20,
                _ => 15
            };
        }

        private EventHandler<EventArgs> _click;

        public event EventHandler<EventArgs> Click
        {
            add
            {
                var notice = _click == null;
                _click += value;
                if (notice)
                {
                    OnPropertyChanged(nameof(HasClick));
                }
            }
            remove
            {
                var notice = _click != null;
                _click -= value;
                if (notice && _click == null)
                {
                    OnPropertyChanged(nameof(HasClick));
                }
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
            get => (ICommand) GetValue(ClickCommandProperty);
            set
            {
                SetValue(ClickCommandProperty, value);
                OnPropertyChanged(nameof(HasClick));
            }
        }

        public ICommand LongClickCommand
        {
            get => (ICommand) GetValue(LongClickCommandProperty);
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

        public bool HasClick => ClickCommand is { } || _click is { };

        public bool HasLongClick => LongClickCommand is { } || _longClick is { };

        public void OnClick(EventArgs args = null)
        {
            ClickCommand?.Execute(null);
            _click?.Invoke(this, args);
        }

        public void OnLongClick(EventArgs args = null)
        {
            LongClickCommand?.Execute(null);
            _longClick?.Invoke(this, args);
        }
    }
}