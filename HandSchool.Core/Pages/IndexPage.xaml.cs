using HandSchool.ViewModels;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : ViewObject
	{
        
        public IndexPage()
        {

            InitializeComponent();
            var today = System.DateTime.Now;
            dayInfo.Text = $"{today.Year}-{today.Month}-{today.Day} {today.DayOfWeek}";
            ViewModel = IndexViewModel.Instance;
            if (Core.Platform.RuntimeName == "Android")
            {
                Content.BackgroundColor = Color.FromRgb(241, 241, 241);
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            //刷新课表之后定位到当前或者下一节课
            System.Threading.Tasks.Task.Run(IndexViewModel.Instance.Refresh).ContinueWith(async (task) =>
            {
                await task;
                var cur = 0;
                var index = -1;
                var vm = ViewModel as IndexViewModel;

                if (!vm.NoClass)
                {
                    foreach (var i in classTable.ItemsSource)
                    {
                        var item = i as Models.CurriculumItem;
                        if (item == vm.CurrentClass)
                        {
                            item.State = Models.ClassState.Current;
                            if (index == -1)
                            {
                                index = cur;
                                item.IsSelected = true;
                            }
                            else item.IsSelected = false;
                        }
                        else if (item == vm.NextClass)
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
                    if (index != -1)
                    {
                        Core.Platform.EnsureOnMainThread(() =>
                        {
                            classTable.ScrollTo(index, position: ScrollToPosition.Center);
                        });
                    }
                };
            });
        }

        protected override void OnDisappearing()
        {
            if ((classTable.ItemsSource as System.Collections.IList).Count != 0)
                classTable.ScrollTo(0, position: ScrollToPosition.Center,animate:false);
            base.OnDisappearing();
        }
        private void ClassTableCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var s = sender as CarouselView;
            foreach(var item in s.VisibleViews)
            {
                var bc = item.BindingContext as Models.CurriculumItem;
                if (bc == null) return;
                if (bc.Equals(e.CurrentItem)) 
                    bc.IsSelected = true;
                else bc.IsSelected = false;
            }
        }
    }
}