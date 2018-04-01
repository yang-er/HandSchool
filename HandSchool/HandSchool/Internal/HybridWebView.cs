using System;
using Xamarin.Forms;

/**
    Thanks to https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/hybridwebview
    https://github.com/xamarin/xamarin-forms-samples/tree/master/CustomRenderers/HybridWebView
    <w:HybridWebView x:Name="hybridWebView" Uri="index.html" HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" />
    hybridWebView.RegisterAction(data => DisplayAlert ("Alert", "Hello " + data, "OK"));

    Author by tlylz99:
    webpg.WebView.JavaScript("$('.table tbody').append('<tr><th scope=\"row\">4</th><td>What</td><td>the</td><td>@fuck</td></tr>')")
 */

namespace HandSchool.Views
{
    public class HybridWebView : View
    {
        Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri", returnType: typeof(string), declaringType: typeof(HybridWebView), defaultValue: default(string));
        public static readonly BindableProperty HtmlProperty = BindableProperty.Create(propertyName: "Html", returnType: typeof(string), declaringType: typeof(HybridWebView), defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public event Action<string> OnExcuteJavaScript;

        public void JavaScript(string str)
        {
            OnExcuteJavaScript?.Invoke(str);
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
    }
}
