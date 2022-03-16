#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HandSchool.Internals
{
    public class ReflectionManager
    {
        public ReflectionManager()
        {
            Assemblies = new List<Assembly>();
            _registeredTypes = new Dictionary<string, Type>();
            _registeredConstructors = new Dictionary<Type, Func<object>>();
            _registeredImpl = new Dictionary<Type, Type>();
        }

        public static ReflectionManager Instance => Lazy.Value;

        private static Lazy<ReflectionManager> Lazy { get; } =
            new Lazy<ReflectionManager>(() => new ReflectionManager());

        public List<Assembly> Assemblies { get; }
        public const string NameSpacePrefix = "HandSchool.";
        private readonly Dictionary<string, Type> _registeredTypes;
        private readonly Dictionary<Type, Func<object>> _registeredConstructors;
        private readonly Dictionary<Type, Type> _registeredImpl;

        public void RegisterFiles(Assembly assembly, string tag, List<string> fileList)
        {
            foreach (var reg in assembly.Gets<RegisterServiceAttribute>())
            {
                var stg = reg.RegisterType.Get<UseStorageAttribute>(false);

                if (stg != null && stg.School == tag)
                {
                    fileList.AddRange(stg.Files);
                }
            }
        }

        /// <summary>
        /// 注册构造方法，以加快反射
        /// </summary>
        /// <typeparam name="T">需要注册构造方法的类的类型，该类型必须含有公开无参构造方法</typeparam>
        /// <returns>此次操作是否更新内部数据结构</returns>
        public bool RegisterConstructor<T>() where T : class, new()
        {
            var type = typeof(T);
            var fullName = type.FullName!;
            if (_registeredTypes.ContainsKey(fullName)) return false;
            _registeredTypes[fullName] = type;
            _registeredConstructors[type] = () => new T();
            _registeredImpl[type] = type;
            return true;
        }

        /// <summary>
        /// 根据Type注册构造方法
        /// </summary>
        /// <returns>此次操作是否更新内部数据结构</returns>
        /// <exception cref="ArgumentException">当类型没有全类名时</exception>
        /// <exception cref="InvalidOperationException">当类型不是有公开无参构造方法的具体类时</exception>
        public bool RegisterConstructor(Type type)
        {
            var fullName = type.FullName ??
                           throw new ArgumentException($"Cannot work because type: {type.Name} has not fullname! ");
            if (_registeredTypes.ContainsKey(fullName)) return false;
            if (!type.IsAbstract && type.GetConstructor(Array.Empty<Type>()) is { } ctor)
            {
                _registeredTypes[fullName] = type;
                _registeredConstructors[type] = () => ctor.Invoke(Array.Empty<object>());
                _registeredImpl[type] = type;
                return true;
            }

            throw new InvalidOperationException(
                $"Cannot register the constructor of this type: {fullName}, because it is abstract or does not have a parameterless constructor. ");
        }

        /// <summary>
        /// 注册实现类，以实现依赖替换
        /// </summary>
        /// <typeparam name="TOri">原始类型（抽象类、接口、简单具体类）</typeparam>
        /// <typeparam name="TImpl">实现类型</typeparam>
        /// <returns>是否为第一次注册</returns>
        public bool RegisterImplement<TOri, TImpl>() where TImpl : class, TOri, new()
        {
            var tOri = typeof(TOri);
            var tImpl = typeof(TImpl);
            var res = !_registeredImpl.ContainsKey(tOri);
            var oriFullName = tOri.FullName!;
            _registeredTypes[oriFullName] = tOri;
            RegisterConstructor<TImpl>();
            _registeredImpl[tOri] = tImpl;
            return res;
        }

        private Type? TryGetRegisteredType(string typeName)
        {
            return _registeredTypes.TryGetValue(typeName, out var res) ? res : null;
        }

        /// <summary>
        /// 通过全类名获取实现类
        /// </summary>
        public Type? TryGetImpl(string typeName)
        {
            return TryGetRegisteredType(typeName) is { } type ? TryGetImpl(type) : null;
        }

        /// <summary>
        /// 通过类型获取实现类
        /// </summary>
        /// <param name="type">原始类型</param>
        /// <returns>若已经注册过实现，则返回注册的实现；若没有注册过实现，则判断该类型自身是否是拥有无参构造方法的具体类，若是，则返回其本身；若不是，则返回Null。</returns>
        public Type? TryGetImpl(Type type)
        {
            if (_registeredImpl.TryGetValue(type, out var impl))
                return impl;
            try
            {
                RegisterConstructor(type);
                return type;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取注册过的构造方法
        /// </summary>
        public Func<object>? TryGetConstructor(Type type)
        {
            return _registeredConstructors.TryGetValue(type, out var ctor) ? ctor : null;
        }

        public Func<object>? TryGetConstructor(string fullName)
        {
            return TryGetRegisteredType(fullName) is { } type ? TryGetConstructor(type) : null;
        }

        /// <summary>
        /// 获取实现类的构造方法
        /// </summary>
        public Func<object>? TryGetImplConstructor(string fullName)
        {
            return TryGetImpl(fullName) is { } type ? TryGetConstructor(type) : null;
        }

        public Func<object>? TryGetImplConstructor(Type type)
        {
            return TryGetImpl(type) is { } implType ? TryGetConstructor(implType) : null;
        }

        public T CreateInstance<T>(Type typeInfo) where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeInfo))
            {
                throw new InvalidOperationException(
                    $"type: {typeInfo.FullName ?? typeInfo.Name} is not a subclass of {typeof(T).FullName ?? typeof(T).Name}");
            }

            if (TryGetConstructor(typeInfo) is { } ctor)
            {
                return (T) ctor.Invoke();
            }

            Core.Logger.WriteLine("CoreRTTI", typeInfo.FullName + " was requested to be activated.");
            return (T) Activator.CreateInstance(typeInfo);
        }

        public T CreateImplement<T>() where T : class
        {
            return TryGetImpl(typeof(T))?.Let(CreateInstance<T>)
                   ?? throw new ArgumentException(
                       $"Cannot find a registered implement type of {typeof(T).FullName ?? typeof(T).Name}");
        }
    }
}