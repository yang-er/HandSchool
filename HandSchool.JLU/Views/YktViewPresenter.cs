using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.Views
{
    internal class YktViewPresenter : IViewPresenter
    {
        private IViewPage CreateHistoryPage()
        {
            var basePage = Core.Platform.CreatePage();
            basePage.ViewModel = YktViewModel.Instance;
            var listView = CreateListView();
            listView.SetBinding(ListView.ItemsSourceProperty, "RecordInfo");
            listView.SetBinding(ListView.RefreshCommandProperty, "RecordFindCommand");
            basePage.Content = listView;
            basePage.Title = HistoryTitle;
            return basePage;
        }

        private IViewPage CreatePickCardPage()
        {
            var basePage = Core.Platform.CreatePage();
            basePage.ViewModel = YktViewModel.Instance;
            var listView = CreateListView();
            listView.SetBinding(ListView.ItemsSourceProperty, "PickCardInfo");
            listView.SetBinding(ListView.RefreshCommandProperty, "LoadPickCardInfoCommand");
            basePage.Content = listView;
            basePage.Title = "拾卡";
            return basePage;
        }
        
        private IViewPage CreateMainPage()
        {
            var basePage = Core.Platform.CreatePage();
            basePage.ViewModel = YktViewModel.Instance;
            var listView = CreateListView();
            listView.SetBinding(ListView.ItemsSourceProperty, "BasicInfo");
            listView.SetBinding(ListView.RefreshCommandProperty, "LoadBasicInfoCommand");
            basePage.Content = listView;
            basePage.Title = MainPageTitle;

            basePage.AddToolbarEntry(new MenuEntry
            {
                Title = "充值",
                Order = ToolbarItemOrder.Primary,
                UWPIcon = "\uEF40",
                CommandBinding = "ChargeCreditCommand"
            });

            basePage.AddToolbarEntry(new MenuEntry
            {
                Title = "挂失",
                Order = ToolbarItemOrder.Primary,
                UWPIcon = "\uE8D7",
                CommandBinding = "SetUpLostStateCommand"
            });

            Task.Run(YktViewModel.Instance.FirstOpen);
            return basePage;
        }

        private string HistoryTitle = "消费记录";
        private string HistoryMessage = "近一周的消费记录信息";
        private string PickCardTitle = "拾卡";
        private string PickCardMessage = "由学校一卡通网站提供的拾卡信息";
        private string MainPageTitle = "概览";

        public int PageCount => Core.Platform.RuntimeName == "iOS" ? 1 : 3;

        private void PushHistory(object obj)
        {
            if (obj is INavigation nav)
            {
                nav.PushAsync(CreateHistoryPage() as Page);
            }
        }

        private void PushPickCard(object obj)
        {
            if (obj is INavigation nav)
            {
                nav.PushAsync(CreatePickCardPage() as Page);
            }
        }

        private void DifferIOS()
        {
            if (PickCardTitle == "拾卡")
            {
                PickCardTitle = "拾卡信息";
                MainPageTitle = "校园一卡通";

                YktViewModel.Instance.BasicInfo.Add(
                    new SchoolCardInfoPiece(
                        PickCardTitle,
                        PickCardMessage,
                        new Command(PushPickCard)
                    ));

                YktViewModel.Instance.BasicInfo.Add(
                    new SchoolCardInfoPiece(
                        HistoryTitle,
                        HistoryMessage,
                        new Command(PushHistory)
                    ));
            }
        }

        public IViewPage[] GetAllPages()
        {
            if (Core.Platform.RuntimeName == "iOS")
            {
                DifferIOS();
                return new[] { CreateMainPage() };
            }
            else
            {
                return new[]
                {
                    CreateMainPage(),
                    CreatePickCardPage(),
                    CreateHistoryPage()
                };
            }
        }

        private Cell CreateCell()
        {
            if (Core.Platform.RuntimeName == "UWP")
            {
                var tc = new TextCell();
                tc.SetBinding(TextCell.TextProperty, "Title");
                tc.SetBinding(TextCell.DetailProperty, "Description");
                return tc;
            }

            var tit = new Label { FontSize = 16 };
            var desc = new Label { FontSize = 13 };
            tit.LineBreakMode = LineBreakMode.WordWrap;
            desc.LineBreakMode = LineBreakMode.WordWrap;
            tit.SetBinding(Label.TextProperty, "Title");
            desc.SetBinding(Label.TextProperty, "Description");
            var layout = new StackLayout { Children = { tit, desc } };
            tit.SetDynamicResource(VisualElement.StyleProperty, "ListItemTextStyle");
            desc.SetDynamicResource(VisualElement.StyleProperty, "ListItemDetailTextStyle");
            layout.Padding = new Thickness(15);
            
            if (Core.Platform.RuntimeName == "iOS")
            {
                var gesture = new TapGestureRecognizer();
                layout.GestureRecognizers.Add(gesture);
                gesture.SetBinding(TapGestureRecognizer.CommandProperty, "Command");
                gesture.CommandParameter = layout.Navigation;
            }

            return new ViewCell { View = layout };
        }

        private ListView CreateListView()
        {
            var listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                SelectionMode = ListViewSelectionMode.None,
                HasUnevenRows = true,
                IsPullToRefreshEnabled = true,
            };

            listView.SetBinding(ListView.IsRefreshingProperty, "IsBusy", BindingMode.OneWay);
            listView.ItemTemplate = new DataTemplate(CreateCell);
            return listView;
        }
    }
}
