using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HandSchool.Internals
{
    public class Thread
    {
        public int ManagedThreadId { get; set; } = 2;

        public static Thread CurrentThread { get; } = new Thread();
    }

    public class ServicePointManager
    {
        public static Predicate<object> ServerCertificateValidationCallback { get; set; }
    }

    public class Trace
    {
        public static void WriteLine(string ans)
        {
            Debug.WriteLine(ans);
        }
    }

    public class AwaredWebClientImpl { }

    public interface ICustomAttributeProvider
    {
        object[] GetCustomAttributes(Type type, bool inherit);
    }

    public static class ReflectionExtensionsCompact
    {
        public static bool IsAssignableFrom(this Type t1, Type t2)
        {
            return TypeExtensions.IsAssignableFrom(t1, t2);
        }

        public static TAttribute Get<TAttribute>(this Type info, bool @checked = true) where TAttribute : Attribute
        {
            return info.GetTypeInfo().Get<TAttribute>(@checked);
        }

        public static IEnumerable<TAttribute> Gets<TAttribute>(this MemberInfo info) where TAttribute : Attribute
        {
            foreach (var obj in info.GetCustomAttributes(typeof(TAttribute), false))
            {
                if (obj is TAttribute attr) yield return attr;
            }
        }

        public static IEnumerable<TAttribute> Gets<TAttribute>(this Assembly info) where TAttribute : Attribute
        {
            foreach (var obj in info.GetCustomAttributes(typeof(TAttribute)))
            {
                if (obj is TAttribute attr) yield return attr;
            }
        }

        public static bool Has<TAttribute>(this Type info) where TAttribute : Attribute
        {
            return info.GetTypeInfo().Has<TAttribute>();
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
            return TypeExtensions.GetProperties(type);
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            return TypeExtensions.GetMethods(type);
        }

        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return TypeExtensions.GetMethod(type, methodName);
        }
    }

    public class MD5CryptoServiceProvider : IDisposable
    {
        public void Dispose() { }

        public byte[] ComputeHash(byte[] source)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var origBuff = WindowsRuntimeBufferExtensions.AsBuffer(source, 0, source.Length);
            var hashed = alg.HashData(origBuff);
            return WindowsRuntimeBufferExtensions.ToArray(hashed, 0, (int)hashed.Length);
        }
    }
    
    public sealed class NavigationViewStateConverter : IValueConverter
    {
        Thickness MinimalMargin { get; } = new Thickness(52, -12, 0, -12);
        Thickness OtherMargin { get; } = new Thickness(12, -12, 0, -12);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is NavigationViewDisplayMode _value)
            {
                return _value == NavigationViewDisplayMode.Minimal ? MinimalMargin : OtherMargin;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
