﻿using HandSchool.Internal;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EntAttr = HandSchool.Services.EntranceAttribute;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WebViewPage : PopContentPage
	{
        private IInfoEntrance InfoEntrance { get; }

		public WebViewPage(IInfoEntrance entrance)
		{
			InitializeComponent();
            var meta = entrance.GetType().GetCustomAttribute(typeof(EntAttr)) as EntAttr;
            Title = meta.Title;
            InfoEntrance = entrance;
            InfoEntrance.Binding = new ViewResponse(this);
            var sb = new StringBuilder();
            InfoEntrance.HtmlDocument.ToHtml(sb);
            WebView.Html = sb.ToString();
            foreach (var key in InfoEntrance.Menu)
            ToolbarItems.Add(new ToolbarItem { Text = key.Name, Command = key.Command });
            entrance.Evaluate = WebView.JavaScript;
            WebView.RegisterAction(entrance.Receive);
        }

        public WebViewPage(AboutViewModel viewModel)
        {
            InitializeComponent();
            Title = viewModel.Title;
            ViewModel = viewModel;
            var sb = new StringBuilder();
            viewModel.HtmlDocument.ToHtml(sb);
            WebView.Html = sb.ToString();
            WebView.RegisterAction(viewModel.Response);
        }
    }
}