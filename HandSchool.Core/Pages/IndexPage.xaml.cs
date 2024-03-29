﻿using System;
using System.Collections;
using System.Threading.Tasks;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndexPage : ViewObject
    {
        private string[] ChineseDayOfWeek = new[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

        public IndexPage()
        {
            InitializeComponent();
            var today = DateTime.Now;
            DayInfo.Text = $"{today.Year}-{today.Month}-{today.Day} {ChineseDayOfWeek[(int)today.DayOfWeek]}";
            ViewModel = IndexViewModel.Instance;
            ClassTable.IndicatorView = ClassTableIndicator;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    UseSafeArea = true;
                    break;
            }
        }

        private void CurrentClassLoadOver(object s, ClassLoadedEventArgs e)
        {
            var cur = 0;
            var index = -1;
            var vm = (IndexViewModel)ViewModel;
            Core.Platform.EnsureOnMainThread(() =>
            {
                IndexViewModel.Instance.ClassToday.Clear();
                
                foreach (var item in e.Classes)
                {
                    IndexViewModel.Instance.ClassToday.Add(item);
                }

                if (!vm.NoClass)
                {
                    foreach (var i in ClassTable.ItemsSource)
                    {
                        var item = (CurriculumItem)i;
                        if (item.State == ClassState.Current || item.State == ClassState.Next)
                        {
                            if (index == -1)
                            {
                                index = cur;
                                item.IsSelected = true;
                            }
                            else item.IsSelected = false;
                        }
                        cur++;
                    }
                }
                Core.Platform.EnsureOnMainThread(async () =>
                {
                    await Task.Yield();
                    if (index != -1)
                    {
                        ClassTable.ScrollTo(index, position: ScrollToPosition.Center,animate:false);
                    }
                });
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IndexViewModel.Instance.CurrentClassesLoadFinished += CurrentClassLoadOver;
            Task.Run(IndexViewModel.Instance.Refresh);
            Task.Run(async () =>
            {
                try
                {
                    await IndexViewModel.Instance.RefreshWeather();
                }
                catch
                {
                    Core.Platform.EnsureOnMainThread(() =>
                    {
                        WeatherFrame.IsVisible = false;
                    });
                }
            });
        }

        protected override void OnDisappearing()
        {
            if (((IList) ClassTable.ItemsSource).Count != 0)
            {
                ClassTable.CurrentItem = ((IndexViewModel)ViewModel).ClassToday[0];
            }
            IndexViewModel.Instance.CurrentClassesLoadFinished -= CurrentClassLoadOver;
            base.OnDisappearing();
        }
        private void ClassTableCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var s = (CarouselView)sender;
            foreach (var item in s.VisibleViews)
            {
                var bc = item.BindingContext as Models.CurriculumItem;
                if (bc == null) return;
                bc.IsSelected = bc.SameAs(e.CurrentItem as CurriculumItemBase);
            }
        }
    }
}