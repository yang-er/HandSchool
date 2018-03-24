using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MessageDetailPage : PopContentPage
	{
		public MessageDetailPage(IMessageItem item)
		{
            Title = "消息详情";
            ToolbarItems.Add(new ToolbarItem() { Text = "删除" });
            Content = new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20),
                Children = {
                    new Label { Text = item.Title, FontSize = 24, TextColor = Color.Black },
                    new Label { Text = "发件人：" + item.Sender, FontSize = 14 },
                    new Label { Text = "时间：" + item.Time.ToString(), FontSize = 14 },
                    new BoxView { Color=Color.Gray, Margin = new Thickness(0,5,0,5), HeightRequest = 1 },
                    new Label { Text = item.Body, FontSize = 16 }
                }
            };
		}
	}
}
