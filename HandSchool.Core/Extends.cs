using HandSchool.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool
{
    public class ClassInfoSimplifier
    {
        private static readonly Lazy<ClassInfoSimplifier> Lazy =
            new Lazy<ClassInfoSimplifier>(Core.New<ClassInfoSimplifier>);

        public static ClassInfoSimplifier Instance => Lazy.Value;
        public virtual string SimplifyName(string str) => str;
    }

    #nullable enable
    public static class ReflectionExtend
    {
        public static FieldInfo? GetDeclaredField(this Type type, string fieldName)
        {
            return type.GetRuntimeFields()?.FirstOrDefault(f => f.Name == fieldName);
        }
        
        public static PropertyInfo? GetDeclaredProperty(this Type type, string propertyName)
        {
            return type.GetRuntimeProperties()?.FirstOrDefault(p => p.Name == propertyName);
        }

        public static MethodInfo? GetDeclaredMethod(this Type type, string methodName, params Type[] types)
        {
            return type.GetRuntimeMethods()?.FirstOrDefault(m =>
            {
                if (m.Name != methodName) return false;
                var mt = m.GetParameters().Select(p => p.ParameterType).ToArray();
                if (mt.Length != types.Length) return false;
                return !mt.Where((t, i) => t != types[i]).Any();
            });
        }
    }

    public static class KotlinExtends
    {
        public static void Let<T>(this T obj, Action<T> action)
        {
            action(obj);
        }

        public static TOut Let<TIn, TOut>(this TIn obj, Func<TIn, TOut> func)
        {
            return func(obj);
        }
    }
    #nullable disable
    
    public static class ColorExtend
    {
        public static Color ColorFromRgb((int, int, int) rgb)
        {
            return Color.FromRgb(rgb.Item1, rgb.Item2, rgb.Item3);
        }

        private static double GetColorNum(double org, double rate) => org * rate > 1 ? 1 : org * rate;

        public static Color ColorDelta(Color color, double rate)
        {
            var rgb = new (int i, double v)[] {(0, color.R), (1, color.G), (2, color.B)};
            Array.Sort(rgb, (a, b) =>
            {
                if (Math.Abs(a.v - b.v) < 1e-6) return 0;
                return a.v < b.v ? -1 : 1;
            });
            rgb[0].v = GetColorNum(rgb[0].v, rate * 0.83);
            rgb[1].v = GetColorNum(rgb[1].v, rate);
            rgb[2].v = GetColorNum(rgb[2].v, rate * 1.2);
            Array.Sort(rgb, (a, b) =>
            {
                if (a.i == b.i) return 0;
                return a.i < b.i ? -1 : 1;
            });
            return Color.FromRgb(rgb[0].v, rgb[1].v, rgb[2].v);
        }
    }

    public static class Extends
    {
        public static bool IsDefaultPort(this Uri uri)
        {
            var should = uri?.Scheme switch
            {
                "http" => 80,
                "https" => 443,
                _ => -1
            };
            return uri?.Port == should;
        }

        public static string GetRootUri(this Uri uri)
        {
            return $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort() ? "" : ":" + uri.Port)}";
        }
        
        public static void SetDefaultFrameCornerRadius(this Frame frame)
        {
            frame.CornerRadius = Device.RuntimePlatform switch
            {
                Device.iOS => 20,
                _ => 15
            };
        }
        public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
        {
            if (list is null || predicate is null) return -1;
            var i = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> enumerable)
        {
            if (enumerable is null) return;
            foreach (var i in enumerable) list.Add(i);
        }

        public static IEnumerable<T> GetReverse<T>(this IEnumerable<T> enumerable)
        {
            var l = enumerable is IList<T> list ? list : enumerable.ToList();
            for (var i = l.Count - 1; i >= 0; i--)
            {
                yield return l[i];
            }
        }

        public static void AddReverse<T>(this IList<T> list, IEnumerable<T> enumerable)
        {
            if (enumerable is null) return;
            list.AddRange(enumerable.Reverse());
        }

        public static Page GetViewObjInstance(Type type, object arg)
        {
            var pageType = Core.Reflection.TryGetType(type);
            if (typeof(ViewObject).IsAssignableFrom(type))
            {
                var vo = Core.Reflection.CreateInstance<ViewObject>(pageType);
                vo.SetNavigationArguments(arg);
                return vo;
            }

            return Core.Reflection.CreateInstance<Page>(pageType);
        }
    }

    public static class VisualElementExtends
    {
        private static class Scales
        {
            public const double Normal = 1;
            public const double Small = 0.90;
            public const double Large = 1.05;
            public const double SuperLarge = 1.1;
        }

        public static async Task TappedAnimation(this VisualElement item, Func<Task> doing = null)
        {
            if (item == null) return;
            await item.ScaleTo(Scales.Small, 200);
            if (doing != null) await doing();
            await item.ScaleTo(Scales.Large, 200);
            await item.ScaleTo(Scales.Normal, 150);
        }

        public static async Task LongPressAnimation(this VisualElement item, Func<Task> doing = null)
        {
            if (item == null) return;
            await item.ScaleTo(Scales.SuperLarge, 200);
            if (doing != null) await doing();
            await item.ScaleTo(Scales.Normal, 200);
        }
    }

    #nullable enable
    public static class CookieContainerExtends
    {
        private static readonly FieldInfo? InnerHashTable;
        private static readonly FieldInfo? InnerCount;

        static CookieContainerExtends()
        {
            var fields = typeof(CookieContainer).GetRuntimeFields();
            if (fields is null) return;
            using var enumerator = fields.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                switch (current?.Name)
                {
                    case "m_domainTable":
                        InnerHashTable = current;
                        break;
                    case "m_count":
                        InnerCount = current;
                        break;
                }
            }
        }

        /// <summary>
        /// 通过反射方式清空cookieContainer
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool Clear(this CookieContainer cookieContainer)
        {
            if (InnerCount is null || InnerHashTable is null) return false;
            if (!(InnerHashTable.GetValue(cookieContainer) is Hashtable ht)) return false;
            ht.Clear();
            InnerCount.SetValue(cookieContainer, 0);
            return true;
        }
    }
}