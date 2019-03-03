using System.Linq;
using HandSchool.Models;
using HandSchool.ViewModels;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradePointPage : ViewObject
    {
        private bool lockedView;

        public GradePointPage()
        {
            InitializeComponent();
            ViewModel = GradePointViewModel.Instance;

            if (Core.Platform.RuntimeName == "Android")
            {
                var ListView = Content as ListView;
                ListView.SeparatorVisibility = SeparatorVisibility.None;
                ListView.Header = new StackLayout { HeightRequest = 4 };
                ListView.Footer = new StackLayout { HeightRequest = 4 };
            }
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var iGi = e.Item as IGradeItem;
            if (iGi is GPAItem) return;
            if (lockedView) return;
            lockedView = true;

            var info = string.Format(
                "名称：{0}\n类型：{1}\n学期：{2}\n发布日期：{3}\n" +
                "学分：{4}\n分数：{5}\n绩点：{6}\n通过：{7}\n重修：{8}",
                iGi.Title, iGi.Type, iGi.Term, iGi.Date.ToString(),
                iGi.Credit, iGi.Score, iGi.Point, iGi.Pass ? "是" : "否", iGi.ReSelect ? "是" : "否");

            foreach (var key in iGi.Attach.Keys)
            {
                info += "\n" + key + "：" + iGi.Attach.Get((string)key);
            }

            await RequestMessageAsync("成绩详情", info, "确定");

            var list = iGi.GetGradeDistribute().ToList();
            if (list.Count > 0)
            {
                var pie = new PieChart { Entries = list, Margin = 10 };
                await RequestChartAsync(pie, "成绩分布");
            }

            lockedView = false;
        }
    }
}