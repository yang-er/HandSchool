#nullable enable
using System;
using System.Linq;
using System.Reflection;
using Android.Views;
using Java.Lang;
using Java.Lang.Reflect;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace HandSchool.Droid.Internals
{
    public class ViewClickListenerGetter
    {
        public bool CanAccess { get; private set; }

        private ViewClickListenerGetter()
        {
            CanAccess = true;
        }

        private static readonly Lazy<ViewClickListenerGetter> Lazy =
            new Lazy<ViewClickListenerGetter>(() => new ViewClickListenerGetter());

        public static ViewClickListenerGetter Instance => Lazy.Value; 
        
        private Method? _method;

        private Field? _field;
        
        private Object? GetListenerObject(View v)
        {
            if (!CanAccess) return null;
            if (_method == null)
            {
                var res = v.Class;
                while (res != null && _method == null)
                {
                    try
                    {
                        _method = res.GetDeclaredMethod("getListenerInfo");
                    }
                    catch (NoSuchMethodException)
                    {
                        res = res.Superclass;
                    }
                }
            }

            if (_method != null)
            {
                _method.Accessible = true;
                try
                {
                    return _method.Invoke(v);
                }
                catch (Exception)
                {
                    CanAccess = false;
                    return null;
                }
            }

            CanAccess = false;
            return null;
        }
        
        /// <summary>
        /// 用反射的方式获取View的ClickListener
        /// </summary>
        public View.IOnClickListener? GetOnClickListener(View v)
        {
            if (!CanAccess) return null;
            var obj = GetListenerObject(v);
            if (obj == null) return null;
            if (_field == null)
            {
                try
                {
                    _field = obj.Class.GetField("mOnClickListener");
                }
                catch (Exception)
                {
                    CanAccess = false;
                    return null;
                }
            }

            try
            {
                _field.Accessible = true;
                return _field.Get(obj) as View.IOnClickListener;
            }
            catch (Exception)
            {
                CanAccess = false;
                return null;
            }
        }
    }
    
    public static class ViewExtends
    {
        /// <summary>
        /// 给被加入的组件加装点击水波纹特效
        /// </summary>
        public static void TryAddRippleAnimation(this View? v)
        {
            if (v is null) return;
            using var tv = new Android.Util.TypedValue();
            v.Context?.Theme?.ResolveAttribute(Resource.Attribute.selectableItemBackground, tv, true);
            v.Foreground ??= v.Context?.Theme?.GetDrawable(tv.ResourceId);
            v.Clickable = true;
            v.LongClickable = true;
        }

        public static FieldInfo? GetRunTimeAllField(this Type? type, string fieldName)
        {
            if (type is null) return null;
            return type.GetRuntimeFields()?.FirstOrDefault(f => f.Name == fieldName) ??
                   GetRunTimeAllField(type.BaseType, fieldName);
        }
    }
}