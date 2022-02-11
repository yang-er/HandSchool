using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public class TouchableFrame : Frame
    {
        public TouchableFrame()
        {
            this.SetDefaultFrameCornerRadius();
            _lastHasClick = HasClick;
            _lastHasLongClick = HasLongClick;
        }

        private EventHandler<EventArgs> _click;

        public event EventHandler<EventArgs> Click
        {
            add
            {
                _click += value;
                if (_lastHasClick == HasClick) return;
                _lastHasClick = HasClick;
                OnPropertyChanged(nameof(HasClick));
            }
            remove
            {
                _click -= value;
                if (_lastHasClick == HasClick) return;
                _lastHasClick = HasClick;
                OnPropertyChanged(nameof(HasClick));
            }
        }

        private EventHandler<EventArgs> _longClick;

        public event EventHandler<EventArgs> LongClick
        {
            add
            {
                _longClick += value;
                if (_lastHasLongClick == HasLongClick) return;
                _lastHasLongClick = HasLongClick;
                OnPropertyChanged(nameof(HasLongClick));
            }
            remove
            {
                _longClick -= value;
                if (_lastHasLongClick == HasLongClick) return;
                _lastHasLongClick = HasLongClick;
                OnPropertyChanged(nameof(HasLongClick));
            }
        }

        public ICommand ClickCommand
        {
            get => (ICommand) GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        public ICommand LongClickCommand
        {
            get => (ICommand) GetValue(LongClickCommandProperty);
            set => SetValue(LongClickCommandProperty, value);
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

        private bool _lastHasClick;

        public bool HasLongClick => LongClickCommand is { } || _longClick is { };

        private bool _lastHasLongClick;

        public void OnClick(EventArgs args = null)
        {
            _click?.Invoke(this, args);
            ClickCommand?.Execute(null);
        }

        public void OnLongClick(EventArgs args = null)
        {
            _longClick?.Invoke(this, args);
            LongClickCommand?.Execute(null);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(ClickCommand):
                {
                    if (HasClick != _lastHasClick)
                    {
                        _lastHasClick = HasClick;
                        OnPropertyChanged(nameof(HasClick));
                    }

                    break;
                }

                case nameof(LongClickCommand):
                {
                    if (HasLongClick != _lastHasLongClick)
                    {
                        _lastHasLongClick = HasLongClick;
                        OnPropertyChanged(nameof(HasLongClick));
                    }

                    break;
                }
            }
        }
    }
}