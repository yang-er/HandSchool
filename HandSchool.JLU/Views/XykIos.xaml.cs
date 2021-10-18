using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using HandSchool.Controls;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XykIos : ViewObject
    {
        private static int _pageCount;

        public XykIos()
        {
            InitializeComponent();
            ViewModel = YktViewModel.Instance;
            charge.Command = YktViewModel.Instance.ChargeCreditCommand;
            lokc.Command = YktViewModel.Instance.SetUpLostStateCommand;
            unlock.Command = new CommandAction(
                async () =>
                {
                    if (await ViewModel.RequestAnswerAsync("提示", "请选择你现在的网络环境，稍后在跳转的网页中操作", "校园网", "公共网络"))
                    {
                        Core.Platform.OpenUrl(
                            "https://vpns.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/homeLogin.action");
                    }
                    else
                    {
                        Core.Platform.OpenUrl("http://xyk.jlu.edu.cn");
                    }
                });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_pageCount != 0) return;
            Task.Run(YktViewModel.Instance.FirstOpen);
            _pageCount = 1;
        }

        //点击之后啥也不干，就是玩
        void ItemTappedHandler(object sender, System.EventArgs args)
        {
            return;
        }

        private async void LoadMoreInfo(System.Object sender, System.EventArgs e)
        {
            if (sender == null) return;
            await Navigation.PushAsync(typeof(XykIosMoreInfo), null);
        }
    }
}