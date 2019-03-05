using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace HandSchool.Design
{
    public abstract class SchoolWrapper : ContainerBuilder
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

        /// <summary>
        /// 学校的加载类。
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="id">简写字母</param>
        /// <param name="config">配置目录</param>
        /// <param name="logger">日志记录器</param>
        protected SchoolWrapper(string name, string id,
            ILogger logger, IConfiguration config, int daily)
        {
            SchoolName = name;
            SchoolId = id;
            Logger = logger;
            Configure = config;
            DailyClassCount = daily;
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
        /// 读取所有数据。
        /// </summary>
        public virtual Task LoadDataAsync()
        {
            return Task.Run(() =>
            {
                Container = Build();
            });
        }

        /// <summary>
        /// 解析依赖项。
        /// </summary>
        /// <typeparam name="T">依赖类型</typeparam>
        /// <returns>依赖项实例</returns>
        public T Resolve<T>()
        {
            if (!Loaded) throw new InvalidOperationException("Container not builded.");
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
    }
}
