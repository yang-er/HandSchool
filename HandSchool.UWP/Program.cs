using System;
using Windows.UI.Xaml;

namespace HandSchool.UWP
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Internal.PlatformImpl.Register();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyLoad += Core.AssemblyLoaded;
            foreach (var sch in Core.GetAvaliableSchools())
                currentDomain.Load(sch);
            Application.Start((p) => new App());
        }
    }
}
