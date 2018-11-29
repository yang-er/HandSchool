using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InVM = HandSchool.ViewModels.IndexViewModel;
using IPDA = HandSchool.UWP.IndexPageDataAdapter;
using ObCol = System.Collections.ObjectModel.ObservableCollection<HandSchool.UWP.GridViewItemData>;

namespace HandSchool.UWP
{
    class IndexPageDataAdapter
    {
        static readonly Lazy<IPDA> lazier = new Lazy<IPDA>(() => new IPDA());

        public static IPDA Instance => lazier.Value;

        readonly GridViewItemData WelcomeBox = new GridViewItemData();

        readonly GridViewItemData NoClassBox = new GridViewItemData
        {
            Icon = Symbol.Clock,
            Line1 = "现在",
            Line2 = "无（没有课或未刷新）",
            Line3 = "享受美好的休息时光吧~"
        };

        readonly GridViewItemData CurrentClassBox = new GridViewItemData
        {
            Icon = Symbol.Clock,
            Line1 = "现在"
        };

        readonly GridViewItemData NextClassBox = new GridViewItemData
        {
            Icon = Symbol.Clock,
            Line1 = "下一节课"
        };

        readonly GridViewItemData WeatherBox = new GridViewItemData
        {
            Icon = Symbol.Send,
            Line1 = "今日天气"
        };

        public ObCol Collection { get; }

        private IndexPageDataAdapter()
        {
            Collection = new ObCol { WelcomeBox, NoClassBox, WeatherBox };
            InVM.Instance.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "CurrentClass")
            {
                CurrentClassBox.Line2 = InVM.Instance.CurrentClass?.Name ?? "";
                CurrentClassBox.Line3 = InVM.Instance.CurrentClass?.Teacher ?? "";
                CurrentClassBox.Line4 = InVM.Instance.CurrentClass?.Classroom ?? "";
            }
            else if (args.PropertyName == "NextClass")
            {
                NextClassBox.Line2 = InVM.Instance.NextClass?.Name ?? "";
                NextClassBox.Line3 = InVM.Instance.NextClass?.Teacher ?? "";
                NextClassBox.Line4 = InVM.Instance.NextClass?.Classroom ?? "";
            }
            else if (args.PropertyName == "Weather")
            {
                WeatherBox.Line2 = InVM.Instance.Weather;
            }
            else if (args.PropertyName == "WeatherRange")
            {
                WeatherBox.Line3 = InVM.Instance.WeatherRange;
            }
            else if (args.PropertyName == "WeatherTips")
            {
                WeatherBox.Line4 = InVM.Instance.WeatherTips;
            }
            else if (args.PropertyName == "WelcomeMessage")
            {
                WelcomeBox.Line1 = InVM.Instance.WelcomeMessage;
            }
            else if (args.PropertyName == "CurrentMessage")
            {
                WelcomeBox.Line2 = InVM.Instance.CurrentMessage;
            }
            else if (args.PropertyName == "NoClass")
            {
                if (InVM.Instance.NoClass)
                {
                    if (!Collection.Contains(NoClassBox))
                    {
                        Collection.Insert(1, NoClassBox);
                    }
                }
                else
                {
                    if (Collection.Contains(NoClassBox))
                    {
                        Collection.Remove(NoClassBox);
                    }
                }
            }
            else if (args.PropertyName == "CurrentHasClass")
            {
                if (InVM.Instance.CurrentHasClass)
                {
                    if (!Collection.Contains(CurrentClassBox))
                    {
                        Collection.Insert(1, CurrentClassBox);
                    }
                }
                else
                {
                    if (Collection.Contains(CurrentClassBox))
                    {
                        Collection.Remove(CurrentClassBox);
                    }
                }
            }
            else if (args.PropertyName == "NextHasClass")
            {
                if (InVM.Instance.NextHasClass)
                {
                    if (!Collection.Contains(NextClassBox))
                    {
                        Collection.Insert(Collection.Contains(CurrentClassBox) ? 2 : 1, NextClassBox);
                    }
                }
                else
                {
                    if (Collection.Contains(NextClassBox))
                    {
                        Collection.Remove(NextClassBox);
                    }
                }
            }
        }
    }

    class GridViewItemData : DependencyObject
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Symbol), typeof(GridViewItemData), new PropertyMetadata(Symbol.Emoji));
        public static readonly DependencyProperty Line1Property =
            DependencyProperty.Register(nameof(Line1), typeof(string), typeof(GridViewItemData), new PropertyMetadata(""));
        public static readonly DependencyProperty Line2Property =
            DependencyProperty.Register(nameof(Line2), typeof(string), typeof(GridViewItemData), new PropertyMetadata(""));
        public static readonly DependencyProperty Line3Property =
            DependencyProperty.Register(nameof(Line3), typeof(string), typeof(GridViewItemData), new PropertyMetadata(""));
        public static readonly DependencyProperty Line4Property =
            DependencyProperty.Register(nameof(Line4), typeof(string), typeof(GridViewItemData), new PropertyMetadata(""));

        public Symbol Icon
        {
            get => (Symbol)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Line1
        {
            get => GetValue(Line1Property) as string;
            set => SetValue(Line1Property, value);
        }

        public string Line2
        {
            get => GetValue(Line2Property) as string;
            set => SetValue(Line2Property, value);
        }

        public string Line3
        {
            get => GetValue(Line3Property) as string;
            set => SetValue(Line3Property, value);
        }

        public string Line4
        {
            get => GetValue(Line4Property) as string;
            set => SetValue(Line4Property, value);
        }
    }
}
