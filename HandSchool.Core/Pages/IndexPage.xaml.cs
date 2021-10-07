using System;
using System.Collections;
using System.Threading.Tasks;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    public class ClassLoadEventArgs : EventArgs
    {
        public int ResIndex = -1;
        public System.Collections.Generic.IList<Models.CurriculumItem> Classes;
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndexPage : ViewObject
    {
        public IndexPage()
        {

            InitializeComponent();
            var today = DateTime.Now;
            dayInfo.Text = $"{today.Year}-{today.Month}-{today.Day} {today.DayOfWeek}";
            ViewModel = IndexViewModel.Instance;
            Content.BackgroundColor = Color.FromRgb(241, 241, 241);
            switch (Device.RuntimePlatform)
            {
                case "iOS":
                    UseSafeArea = true;
                    break;
            }
        }

        private void CurrentClassLoadOver(object s, ClassLoadEventArgs e)
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
                    foreach (var i in classTable.ItemsSource)
                    {
                        var item = (Models.CurriculumItem)i;
                        if (ReferenceEquals(item, vm.CurrentClass))
                        {
                            item.State = Models.ClassState.Current;
                            if (index == -1)
                            {
                                index = cur;
                                item.IsSelected = true;
                            }
                            else item.IsSelected = false;
                        }
                        else if (ReferenceEquals(item,vm.NextClass))
                        {
                            item.State = Models.ClassState.Next;
                            if (index == -1)
                            {
                                index = cur;
                                item.IsSelected = true;
                            }
                            else item.IsSelected = false;
                        }
                        else item.State = Models.ClassState.Other;
                        cur++;
                    }
                }
                Core.Platform.EnsureOnMainThread(async () =>
                {
                    await Task.Yield();
                    if (index != -1)
                    {
                        classTable.ScrollTo(index, position: ScrollToPosition.Center,animate:false);
                    }
                });
            });
        }

        private bool _isWorking = false;
        private readonly TimeoutManager _weatherTimeoutManager = new TimeoutManager(3600);

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IndexViewModel.Instance.CurrentClassesLoadFinished += CurrentClassLoadOver;
            Task.Run(IndexViewModel.Instance.Refresh);
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    if (!_isWorking && (_weatherTimeoutManager.NotInit || _weatherTimeoutManager.IsTimeout()))
                    {
                        Task.Run(async () =>
                        {
                            await Task.Yield();
                            _isWorking = true;

                            try
                            {
                                var weatherClient = IndexViewModel.Instance.WeatherClient;
                                await weatherClient.UpdateWeatherAsync();
                            
                                Core.Platform.EnsureOnMainThread(() =>
                                {
                                    try
                                    {
                                        CurrentWeather.Text =
                                            $"{weatherClient.CurrentTemperature.value}{weatherClient.CurrentTemperature.unit} {weatherClient.WeatherDescription}";
                                        var report = weatherClient.WeatherDescriptions;
                                        TodayWeather.Text =
                                            $"{weatherClient.ForecastTemperature.value[0].@from}{weatherClient.ForecastTemperature.unit} ~ {weatherClient.ForecastTemperature.value[0].to}{weatherClient.ForecastTemperature.unit} {(report[0].IsFromEqualsTo() ? report[0].@from : $"{report[0].@from}转{report[0].to}")}";
                                        TomorrowWeather.Text =
                                            $"{weatherClient.ForecastTemperature.value[1].@from}{weatherClient.ForecastTemperature.unit} ~ {weatherClient.ForecastTemperature.value[1].to}{weatherClient.ForecastTemperature.unit} {(report[1].IsFromEqualsTo() ? report[1].@from : $"{report[1].@from}转{report[1].to}")}";
                                        weather.IsVisible = true;
                                        _weatherTimeoutManager.Refresh();
                                    }
                                    catch(Exception error)
                                    {
                                        Core.Logger.WriteLine("天气信息与UI同步错误", error.Message);
                                        weather.IsVisible = false;
                                    }
                                });
                            }
                            catch(Exception e)
                            {
                                Core.Logger.WriteLine("更新天气错误", e.Message);
                                Core.Platform.EnsureOnMainThread(() => weather.IsVisible = false);
                            }
                            finally
                            {
                                _isWorking = false;
                            }
                        });
                    }
                    break;
                }
                default: weather.IsVisible = false;
                    break;
            }
        }

        protected override void OnDisappearing()
        {
            if (((IList) classTable.ItemsSource).Count != 0)
            {
                classTable.CurrentItem = ((IndexViewModel)ViewModel).ClassToday[0];
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
                bc.IsSelected = bc.Equals(e.CurrentItem);
            }
        }
    }
}