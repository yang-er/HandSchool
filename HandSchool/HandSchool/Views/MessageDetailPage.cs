using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MessageDetailPage : PopContentPage
	{
		public MessageDetailPage(IMessageItem item)
		{
            Title = "消息详情";
            ToolbarItems.Add(new ToolbarItem() { Text = "删除" });
            Content = new StackLayout {
                Spacing = 10,
                Padding = new Thickness(20),
                Children = {
                    new Label { Text = item.Title, FontSize = 24, TextColor = Color.Black },
                    new Label { Text = item.Time.ToString(), FontSize = 14 },
                    new Label { Text = item.Body, FontSize = 16 }
                }
			};
		}
	}
}
