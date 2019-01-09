using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP
{
    internal class IndexPageDataAdapter
    {
        static readonly Lazy<IndexPageDataAdapter> lazier =
            new Lazy<IndexPageDataAdapter>(() => new IndexPageDataAdapter());

        public static IndexPageDataAdapter Instance => lazier.Value;

        readonly GridViewItemData WelcomeBox = new GridViewItemData
        {
            Line1 = "欢迎",
            Line2 = "正在加载……",
        };

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

        public ObservableCollection<GridViewItemData> Collection { get; }

        private IndexPageDataAdapter()
        {
            Collection = new ObservableCollection<GridViewItemData> { WelcomeBox, NoClassBox, WeatherBox };
            IndexViewModel.Instance.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "CurrentClass")
            {
                CurrentClassBox.Line2 = IndexViewModel.Instance.CurrentClass?.Name ?? "";
                CurrentClassBox.Line3 = IndexViewModel.Instance.CurrentClass?.Teacher ?? "";
                CurrentClassBox.Line4 = IndexViewModel.Instance.CurrentClass?.Classroom ?? "";
            }
            else if (args.PropertyName == "NextClass")
            {
                NextClassBox.Line2 = IndexViewModel.Instance.NextClass?.Name ?? "";
                NextClassBox.Line3 = IndexViewModel.Instance.NextClass?.Teacher ?? "";
                NextClassBox.Line4 = IndexViewModel.Instance.NextClass?.Classroom ?? "";
            }
            else if (args.PropertyName == "Weather")
            {
                WeatherBox.Line2 = IndexViewModel.Instance.Weather;
            }
            else if (args.PropertyName == "WeatherRange")
            {
                WeatherBox.Line3 = IndexViewModel.Instance.WeatherRange;
            }
            else if (args.PropertyName == "WeatherTips")
            {
                WeatherBox.Line4 = IndexViewModel.Instance.WeatherTips;
            }
            else if (args.PropertyName == "WelcomeMessage")
            {
                WelcomeBox.Line1 = IndexViewModel.Instance.WelcomeMessage;
            }
            else if (args.PropertyName == "CurrentMessage")
            {
                WelcomeBox.Line2 = IndexViewModel.Instance.CurrentMessage;
            }
            else if (args.PropertyName == "NoClass")
            {
                if (IndexViewModel.Instance.NoClass)
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
                if (IndexViewModel.Instance.CurrentHasClass)
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
                if (IndexViewModel.Instance.NextHasClass)
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
}
