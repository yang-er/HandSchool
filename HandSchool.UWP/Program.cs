using Windows.UI.Xaml;

namespace HandSchool.UWP
{
    public static class Program
    {
        static void Main(string[] args)
        {
            PlatformImpl.Register();
#if DEBUG
            Core.Reflection.ForceLoad(true);
#else
            Core.Reflection.ForceLoad(false);
#endif
            Application.Start((p) => new App());
        }
    }
}
