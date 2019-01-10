using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HandSchool.Internal
{
    /// <summary>
    /// 处理反射相关的函数，例如获取属性与创建类实例。
    /// </summary>
    public class ReflectionManager
    {
        /// <summary>
        /// 反射管理的单例实现
        /// </summary>
        public static Lazy<ReflectionManager> Lazy { get; } =
            new Lazy<ReflectionManager>(() => new ReflectionManager());

        /// <summary>
        /// 所有相关程序集
        /// </summary>
        public List<Assembly> Assemblies { get; }

        /// <summary>
        /// 命名空间前缀
        /// </summary>
        public const string NameSpacePrefix = "HandSchool.";

        /// <summary>
        /// 创建一个反射管理器，并保存相关内容。
        /// </summary>
        private ReflectionManager()
        {
            Assemblies = new List<Assembly>();
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoaded;
        }

        /// <summary>
        /// 当程序集加载时，检查是否是学校对应的代码。
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">包含了程序集的参数</param>
        public void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.FullName.StartsWith(NameSpacePrefix))
            {
                Assemblies.Add(args.LoadedAssembly);
                CheckForSchool(args.LoadedAssembly);
            }
        }

        /// <summary>
        /// 获取所有的程序集并尝试载入。
        /// </summary>
        public void ForceLoad(bool intended = false)
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.FullName.StartsWith(NameSpacePrefix))
                {
                    Assemblies.Add(item);
                    CheckForSchool(item);
                }
            }

            if (intended)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var file in Directory.EnumerateFiles(baseDir, "HandSchool.*.dll"))
                {
                    var fileShort = file.Replace(baseDir, "");
                    var assemblyName = fileShort.Replace(".dll", "");
                    AppDomain.CurrentDomain.Load(new AssemblyName(assemblyName));
                }
            }
        }

        /// <summary>
        /// 检查程序集是否为保存了学校信息，如果是则加载。
        /// </summary>
        /// <param name="assembly">检查的程序集</param>
        private void CheckForSchool(Assembly assembly)
        {
            var export = assembly.GetCustomAttribute<ExportSchoolAttribute>();
            if (export is null) return;
            var loader = CreateInstance<ISchoolWrapper>(export.RegisterType);
            Core.Schools.Add(loader);
        }

        /// <summary>
        /// 加载程序集注册的文件。
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterFiles(Assembly assembly, List<string> fileList)
        {
            foreach (var reg in assembly.Gets<RegisterServiceAttribute>())
            {
                var stg = reg.RegisterType.Get<UseStorageAttribute>(false);

                if (stg != null && stg.School == "JLU")
                {
                    fileList.AddRange(stg.Files);
                }
            }
        }

        /// <summary>
        /// 创建一个对象的实例。
        /// </summary>
        /// <typeparam name="T">实例化类型</typeparam>
        /// <param name="typeInfo">需要实例化的类型</param>
        /// <returns>实例对象</returns>
        public T CreateInstance<T>(Type typeInfo) where T : class
        {
            Core.Logger.WriteLine("CoreRTTI", typeInfo.FullName + " was requested to be activated.");
            Debug.Assert(typeof(T).IsAssignableFrom(typeInfo));
            return Activator.CreateInstance(typeInfo) as T;
        }
    }
}
