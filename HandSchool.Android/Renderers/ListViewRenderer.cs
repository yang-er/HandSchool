using System.ComponentModel;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRenderer2))]
namespace HandSchool.Droid.Renderers
{
    public class ListViewRenderer2 : ListViewRenderer
    {
        public ListViewRenderer2(Context context) : base(context) { }
        
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                e.NewElement.SelectionMode = ListViewSelectionMode.None;

                if (e.NewElement.Header is StackLayout stackLayout)
                {
                    if (stackLayout.HeightRequest == 4)
                    {
                        // Only items that use CardView would reach here
                        Control.SetSelector(Android.Resource.Color.Transparent);
                    }
                }
            }
        }
    }
}