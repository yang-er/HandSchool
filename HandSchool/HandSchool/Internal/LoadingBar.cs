using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform;

namespace HandSchool.Views
{
    [RenderWith(typeof(_LoadingBarRenderer))]
    public class LoadingBar : View, IElementConfiguration<LoadingBar>
    {
        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create("IsRunning", typeof(bool), typeof(LoadingBar), default(bool));
        public static readonly BindableProperty ColorProperty = BindableProperty.Create("Color", typeof(Color), typeof(LoadingBar), Color.Default);
        
        //readonly Lazy<PlatformConfigurationRegistry<LoadingBar>> _platformConfigurationRegistry;
        
        public LoadingBar()
        {
            //_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<LoadingBar>>(() => new PlatformConfigurationRegistry<LoadingBar>(this));
        }
        
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        
        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }

        public IPlatformElementConfiguration<T, LoadingBar> On<T>() where T : IConfigPlatform
        {
            return null;
            //return _platformConfigurationRegistry.Value.On<T>();
        }
    }
    
    internal class _LoadingBarRenderer { }
}