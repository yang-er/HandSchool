using HandSchool.Views;

namespace HandSchool.Internals
{
    public class Registry
    {
        public static void RegisterTypes()
        {
            Core.Reflection.RegisterCtor<GradePointPage>();
            Core.Reflection.RegisterCtor<FeedPage>();
            Core.Reflection.RegisterCtor<InfoQueryPage>();
            Core.Reflection.RegisterCtor<MessagePage>();
            Core.Reflection.RegisterCtor<DetailPage>();
            Core.Reflection.RegisterCtor<SchedulePage>();
            Core.Reflection.RegisterCtor<SettingPage>();
            Core.Reflection.RegisterCtor<SelectTypePage>();
            Core.Reflection.RegisterCtor<WelcomePage>();
        }
    }
}