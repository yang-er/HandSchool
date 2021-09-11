using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XykIOS : ViewObject
    {
        static int PageCount;
        public XykIOS()
		{
			InitializeComponent();
            ViewModel = YktViewModel.Instance;
            charge.Command = YktViewModel.Instance.ChargeCreditCommand;
            lokc.Command = YktViewModel.Instance.SetUpLostStateCommand;
            unlock.Command = new Command(async()=> {
                if(await ViewModel.RequestAnswerAsync("提示", "请选择你现在的网络环境，稍后在跳转的网页中操作", "校园网", "公共网络"))
                {
                    Core.Platform.OpenUrl("https://vpns.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/homeLogin.action");
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

            //这里分平台是因为iOS平台的回收比较变态，会导致第二次进入仍然执行FirstOpen
            if (PageCount == 0)
            {
                Task.Run(YktViewModel.Instance.FirstOpen);
                PageCount = 1;
            }
        }

        //点击之后啥也不干，就是玩
        async void ItemTappedHandler(object sender, System.EventArgs args)
        {
            await(sender as VisualElement).TappedAnimation(null);
        }

        async void QandA_Clicked(System.Object sender, System.EventArgs e)
        {
            if (sender == null) return;
            var action = await ViewModel.RequestActionAsync("更多", "取消", null, "账户信息", "常见问题");
            if (action.Contains("常见问题"))
            {
                await Navigation.PushAsync(typeof(XykIOS_QandA), null);
            }
            else if (action.Contains("账户信息"))
            {
                await Navigation.PushAsync(typeof(XykIOS_UserInfo), null);
            }

        }
    }
}