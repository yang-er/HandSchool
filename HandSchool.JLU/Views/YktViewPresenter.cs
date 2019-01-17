using HandSchool.Views;

namespace HandSchool.JLU.Views
{
    internal class YktViewPresenter : IViewPresenter
    {
        private string HistoryTitle = "消费记录";
        private string HistoryMessage = "近一周的消费记录信息";
        private string PickCardTitle = "拾卡";
        private string PickCardMessage = "由学校一卡通网站提供的拾卡信息";
        private string MainPageTitle = "概览";

        public int PageCount => Core.Platform.RuntimeName == "iOS" ? 1 : 3;

        public string Title => "校园一卡通";
        
        public IViewPage[] GetAllPages()
        {
            if (Core.Platform.RuntimeName == "iOS")
            {
                return new[] { new YktMainPage { Title = "校园一卡通" } };
            }
            else
            {
                return new IViewPage[]
                {
                    new YktMainPage { Title = "概览" },
                    new YktPickPage { Title = "拾卡" },
                    new YktHistoryPage { Title = "消费记录" },
                };
            }
        }
    }
}