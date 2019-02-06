using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HandSchool.Internals
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
            Registar = new Dictionary<string, Type>();
            ImplRegistar = new Dictionary<string, Type>();
            CtorRegistar = new Dictionary<string, Func<object>>();
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
        /// 添加注册的类型信息
        /// </summary>
        private Dictionary<string, Type> Registar { get; }

        /// <summary>
        /// 各个实现的注册类
        /// </summary>
        private Dictionary<string, Type> ImplRegistar { get; }

        /// <summary>
        /// 各个实现的注册类
        /// </summary>
        private Dictionary<string, Func<object>> CtorRegistar { get; }

        /// <summary>
        /// 注册类型信息，以供反射使用。
        /// </summary>
        /// <typeparam name="T">实际对应的类型</typeparam>
        public void RegisterCtor<T>() where T : new()
        {
            var typed = typeof(T);
            if (Registar.ContainsKey(typed.FullName)) return;
            Registar.Add(typed.FullName, typed);
            Registar.Add(typed.Name, typed);
            CtorRegistar.Add(typed.FullName, () => new T());
        }
        
        /// <summary>
        /// 注册替换实现内容，以供反射使用。
        /// </summary>
        /// <typeparam name="T1">抽象类型</typeparam>
        /// <typeparam name="T2">实现类型</typeparam>
        public void RegisterType<T1, T2>()
        {
            var typed = typeof(T1);
            var typee = typeof(T2);
            Registar[typed.FullName] = typee;
            Registar[typed.Name] = typee;
        }

        /// <summary>
        /// 尝试获得注册的类型信息。
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型信息</returns>
        public Type TryGetType(string typeName)
        {
            return Registar.ContainsKey(typeName) ? Registar[typeName] : null;
        }

        /// <summary>
        /// 尝试获得注册的类型信息。
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型信息</returns>
        public Type TryGetType(Type type)
        {
            return Registar.ContainsKey(type.FullName) ? Registar[type.FullName] : type;
        }

        /// <summary>
        /// 创建一个对象的实例。
        /// </summary>
        /// <typeparam name="T">实例化类型</typeparam>
        /// <param name="typeInfo">需要实例化的类型</param>
        /// <returns>实例对象</returns>
        public T CreateInstance<T>(Type typeInfo) where T : class
        {
            Debug.Assert(typeof(T).IsAssignableFrom(typeInfo));

            if (CtorRegistar.ContainsKey(typeInfo.FullName))
            {
                return CtorRegistar[typeInfo.FullName].Invoke() as T;
            }
            else
            {
                Core.Logger.WriteLine("CoreRTTI", typeInfo.FullName + " was requested to be activated.");
                return Activator.CreateInstance(typeInfo) as T;
            }
        }

        /// <summary>
        /// 创建一个抽象类型的实现。
        /// </summary>
        /// <typeparam name="T">抽象类型</typeparam>
        /// <returns>实现对象</returns>
        public T CreateInstance<T>() where T : class
        {
            var guid = typeof(T).FullName;
            Debug.Assert(Registar.ContainsKey(guid));
            return CreateInstance<T>(Registar[guid]);
        }
    }
}