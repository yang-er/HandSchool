using Autofac;
using HandSchool.Design.Logging;
using HandSchool.Internals;
using System;
using System.Collections.Generic;

namespace HandSchool.Design.Lifecycle
{
    public static class ResolveRootExtensions
    {
        public static Core UseFormsView(this Core that)
        {
            that.Register(c => new Views.WelcomePage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.SettingPage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.SelectTypePage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.SchedulePage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.MessagePage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.InfoQueryPage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.IndexPage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.GradePointPage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.FeedPage())
                .InstancePerLifetimeScope();
            that.Register(c => new Views.DetailPage())
                .InstancePerLifetimeScope();
            return that;
        }

        public static Core UseHttpClient(this Core that)
        {
            that.Register(c => new HttpClientImpl())
                .As<IWebClient>()
                .InstancePerDependency();
            return that;
        }
        
        public static Core UsePage<T>(this Core that)
            where T : Views.IViewPage, new()
        {
            that.Register(c => new T())
                .InstancePerLifetimeScope();
            that.Registar.Add(typeof(T).Name, typeof(T));
            return that;
        }

        public static Core UseLogger(this Core that)
        {
            ILogger tl = new TraceLogger();
            that.RegisterInstance(tl);
            that.RegisterGeneric(typeof(NestedLogger<>))
                .As(typeof(ILogger<>));
            return that;
        }
        
        public static Core UseLoginPage<T>(this Core that)
            where T : Views.ILoginPage, new()
        {
            that.Register(c => new T())
                .As<Views.ILoginPage>()
                .InstancePerDependency();
            return that;
        }

        public static Core UseCurriculumPage<T>(this Core that)
            where T : Views.ICurriculumPage, new()
        {
            that.Register(c => new T())
                .As<Views.ICurriculumPage>()
                .InstancePerDependency();
            return that;
        }

        public static Core UseWebViewPage<T>(this Core that)
            where T : Views.IWebViewPage, new()
        {
            that.Register(c => new T())
                .As<Views.IWebViewPage>()
                .InstancePerDependency();
            return that;
        }
    }
}
