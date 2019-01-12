namespace HandSchool.Views
{
    public class MessagePresenter : IViewPresenter
    {
        public int PageCount => 2;

        public string Title => "消息通知";

        public IViewPage[] GetAllPages()
        {
            return new IViewPage[]
            {
                new FeedPage(),
                new MessagePage()
            };
        }
    }
}