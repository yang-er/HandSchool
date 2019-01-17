using HandSchool.Internal;
using HandSchool.Services;
using System;
using System.IO;
using System.Reflection;

namespace HandSchool.Forwarder
{
    /// <summary>
    /// 实现了反射型加载的类。
    /// </summary>
    public static class ReflectWay
    {
        /// <summary>
        /// 开始通过反射的方式加载。
        /// </summary>
        public static void Begin()
        {
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoaded;
#if DEBUG
            ForceLoad(true);
#else
            ForceLoad(false);
#endif
        }
        
        /// <summary>
        /// 当程序集加载时，检查是否是学校对应的代码。
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">包含了程序集的参数</param>
        private static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.FullName.StartsWith(ReflectionManager.NameSpacePrefix))
            {
                Core.Reflection.Assemblies.Add(args.LoadedAssembly);
                CheckForSchool(args.LoadedAssembly);
            }
        }

        /// <summary>
        /// 获取所有的程序集并尝试载入。
        /// </summary>
        private static void ForceLoad(bool intended = false)
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.FullName.StartsWith(ReflectionManager.NameSpacePrefix))
                {
                    Core.Reflection.Assemblies.Add(item);
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
        private static void CheckForSchool(Assembly assembly)
        {
            var export = assembly.GetCustomAttribute<ExportSchoolAttribute>();
            if (export is null) return;
            var loader = Core.Reflection.CreateInstance<ISchoolWrapper>(export.RegisterType);
            Core.Schools.Add(loader);
        }
    }
}