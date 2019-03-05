using Autofac;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Design
{
    public abstract class SchoolBuilder : ContainerBuilder, IDisposable
    {
        /// <summary>
        /// 学校的名称
        /// </summary>
        public string SchoolName { get; }

        /// <summary>
        /// 学校的简写字母
        /// </summary>
        public string SchoolId { get; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// 设置目录
        /// </summary>
        public IConfiguration Configure { get; }

        /// <summary>
        /// 每日课程数目
        /// </summary>
        public int DailyClassCount { get; }

        /// <summary>
        /// 依赖注入生成的容器
        /// </summary>
        protected IContainer Container { get; set; }

        /// <summary>
        /// 是否已经加载完成
        /// </summary>
        public bool Loaded => Container != null;

        private readonly Lazy<List<string>> lazyRegFiles;

        private readonly Lazy<List<SettingWrapper>> lazySettings;

        /// <summary>
        /// 学校的加载类。
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="id">简写字母</param>
        /// <param name="config">配置目录</param>
        /// <param name="logger">日志记录器</param>
        protected SchoolBuilder(string name, string id,
            ILogger logger, IConfiguration config, int daily)
        {
            SchoolName = name;
            SchoolId = id;
            Logger = logger;
            Configure = config;
            DailyClassCount = daily;

            lazyRegFiles = new Lazy<List<string>>(GetRegisteredFiles);
            lazySettings = new Lazy<List<SettingWrapper>>(EnumerateSettings);
        }

        /// <summary>
        /// 获取已经注册的文件。
        /// </summary>
        /// <returns>文件列表</returns>
        protected List<string> GetRegisteredFiles()
        {
            var fileList = new List<string>();

            foreach (var reg in this.GetAssembly().Gets<RegisterServiceAttribute>())
            {
                var stg = reg.RegisterType.Get<UseStorageAttribute>(false);

                if (stg != null && stg.School == "JLU")
                {
                    fileList.AddRange(stg.Files);
                }
            }

            return fileList;
        }

        /// <summary>
        /// 启动服务。
        /// </summary>
        public virtual void Startup()
        {
            this.Register(c => Logger);

            this.RegisterGeneric(typeof(NestedLogger<>))
                .As(typeof(ILogger<>))
                .OnActivated(e => ((ILogger)e.Instance).Info("instance created."));

            this.Register(c => new Internals.HttpClientImpl())
                .As<Internals.IWebClient>();
        }

        /// <summary>
        /// 获取依赖注入容器并启动数据加载任务。
        /// </summary>
        public void BuildStartup()
        {
            Container = Build();
            Task.Run(LoadDataAsync);
        }
        
        /// <summary>
        /// 读取所有数据。
        /// </summary>
        public virtual async Task LoadDataAsync()
        {
            try
            {
                await Task.Yield();
                await Task.Run(() => Resolve<ISchoolSystem>());
                await Resolve<FeedViewModel>().LoadCacheAsync();
                await Resolve<GradePointViewModel>().LoadCacheAsync();
                await Task.Run(() => Resolve<ScheduleViewModel>().FindItem(p => true));
            }
            catch (Exception ex)
            {
                Logger.Error("Unhandled exception. May occur bad things.");
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// 使用的储存文件。将在重置时清除。
        /// </summary>
        public List<string> RegisteredFiles => lazyRegFiles.Value;

        /// <summary>
        /// 列举所有的设置。
        /// </summary>
        /// <returns></returns>
        protected virtual HeadedList<SettingWrapper> EnumerateSettings()
        {
            return new HeadedList<SettingWrapper>("学校设置");
        }

        /// <summary>
        /// 学校相关的所有设置
        /// </summary>
        public IList<SettingWrapper> Settings => lazySettings.Value;

        /// <summary>
        /// 解析依赖项。
        /// </summary>
        /// <typeparam name="T">依赖类型</typeparam>
        /// <returns>依赖项实例</returns>
        public T Resolve<T>()
        {
            if (!Loaded) throw new InvalidOperationException("Container not built.");
            return Container.Resolve<T>();
        }
        
        /// <summary>
        /// 查询依赖项是否已被提供。
        /// </summary>
        /// <typeparam name="T">依赖项</typeparam>
        /// <returns>是否被提供</returns>
        public bool Provided<T>()
        {
            if (!Loaded) return false;
            return Container.IsRegistered<T>();
        }

        /// <summary>
        /// 清理依赖注入的容器和其中的对象。
        /// </summary>
        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}
