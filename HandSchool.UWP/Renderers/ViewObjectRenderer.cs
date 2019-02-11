using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace HandSchool.UWP.Renderers
{
    public class ViewObjectRenderer : VisualElementRenderer<Page, FrameworkElement>
    {
        bool _disposed;
        
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            // Pages need an automation peer so we can interact with them in automated tests
            return new FrameworkElementAutomationPeer(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _disposed = true;

            if (Element != null)
            {
                var children = ((IElementController)Element).LogicalChildren;
                for (var i = 0; i < children.Count; i++)
                {
                    if (children[i] is VisualElement visualChild) Cleanup(visualChild);
                }
                Element?.SendDisappearing();
            }

            base.Dispose();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                if (e.OldElement == null)
                {
                    Tracker = new BackgroundTracker2<FrameworkElement>(BackgroundProperty);
                }

                if (!string.IsNullOrEmpty(Element.AutomationId))
                {
                    SetAutomationId(Element.AutomationId);
                }
            }
        }
        
        internal static void Cleanup(VisualElement self)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            
            var renderer = Platform.GetRenderer(self);
            
            foreach (Element element in self.Descendants())
            {
                if (!(element is VisualElement visual))
                    continue;

                var childRenderer = Platform.GetRenderer(visual);

                if (childRenderer != null)
                {
                    childRenderer.Dispose();
                    Platform.SetRenderer(visual, null);
                }
            }
            
            if (renderer != null)
            {
                renderer.Dispose();
                Platform.SetRenderer(self, null);
            }
        }
    }
}