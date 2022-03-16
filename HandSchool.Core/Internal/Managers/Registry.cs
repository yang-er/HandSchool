using HandSchool.Services;
using HandSchool.Views;

namespace HandSchool.Internals
{
    public class Registry
    {
        public static void RegisterTypes()
        {
            Core.Reflection.RegisterConstructor<IndexPage>();
            Core.Reflection.RegisterConstructor<GradePointPage>();
            Core.Reflection.RegisterConstructor<FeedPage>();
            Core.Reflection.RegisterConstructor<InfoQueryPage>();
            Core.Reflection.RegisterConstructor<MessagePage>();
            Core.Reflection.RegisterConstructor<DetailPage>();
            Core.Reflection.RegisterConstructor<SchedulePage>();
            Core.Reflection.RegisterConstructor<SettingPage>();
            Core.Reflection.RegisterConstructor<WelcomePage>();
            Core.Reflection.RegisterConstructor<SelectTypePage>();
            Core.Reflection.RegisterConstructor<ClassInfoSimplifier>();
            Core.Reflection.RegisterImplement<IWeatherReport, DefaultWeatherReport>();
        }
    }
}