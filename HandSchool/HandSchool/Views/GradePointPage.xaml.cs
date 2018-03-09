using HandSchool.Internal;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradePointPage : ContentPage
    {
        public ObservableCollection<IGradeItem> Items => Grade.Value;
        public ISystemEntrance Entrance;

        public GradePointPage()
        {
            InitializeComponent();
            Entrance = new JLU.GradeEntrance() { RowLimit = 15 };
            Entrance.Execute();
			MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var iGi = e.Item as IGradeItem;
            var info = string.Format(
                "名称：{0}\n类型：{1}\n学期：{2}\n发布日期：{3}\n" +
                "学分：{4}\n分数：{5}\n绩点：{6}\n通过：{7}\n重修：{8}",
                iGi.Name, iGi.Type, iGi.Term, iGi.Date.ToString(), 
                iGi.Credit, iGi.Score, iGi.Point, iGi.Pass ? "是" : "否", iGi.ReSelect ? "是" : "否");
            foreach (var key in iGi.Attach.Keys)
            {
                info += "\n" + key + "：" + iGi.Attach.Get((string)key);
            }
            await DisplayAlert("成绩详情", info, "确定");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
