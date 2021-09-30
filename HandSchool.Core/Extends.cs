﻿using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool
{
    public class ClassInfoSimplifier
    {
        private static Lazy<ClassInfoSimplifier> lazy = new Lazy<ClassInfoSimplifier>(() => Core.New<ClassInfoSimplifier>());
        public static ClassInfoSimplifier Instance => lazy.Value;
        public virtual string SimplifyName(string str) => str;
    }
    public static class TextAtomScales
    {
        public static readonly double Normal = 0.95;
        public static readonly double Small = 0.95 * 0.90;
        public static readonly double Large = 0.95 * 1.05;
        public static readonly double SuperLarge = 0.95 * 1.1;
    }
    public static class ColorExtend
    {
        public static Color ColorFromRgb((int, int, int) rgb)
        {
            return Color.FromRgb(rgb.Item1, rgb.Item2, rgb.Item3);
        }
        static double GetColorNum(double org, double rate) => org * rate > 1 ? 1 : org * rate;
        public static Color ColorDelta(Color color, double rate)
        {
            var rgb = new (int i, double v)[] { (0, color.R), (1, color.G), (2, color.B) };
            Array.Sort(rgb, (a, b) =>
            {
                if (a.v == b.v) return 0;
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
        public async static Task TappedAnimation(this VisualElement item, Func<Task> doing = null)
        {
            if (item == null) return;
            await item.ScaleTo(TextAtomScales.Small, 200);
            if (doing != null) await doing();
            await item.ScaleTo(TextAtomScales.Large, 200);
            await item.ScaleTo(TextAtomScales.Normal, 150);
        }
        public async static Task LongPressAnimation(this VisualElement item, Func<Task> doing = null)
        {
            if (item == null) return;
            await item.ScaleTo(TextAtomScales.SuperLarge, 200);
            if (doing != null) await doing();
            await item.ScaleTo(TextAtomScales.Normal, 200);
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
    public class LoginTimeoutManager
    {
        public DateTime? LastLoginTime { get; protected set; }
        public readonly int TimeoutSec;
        public LoginTimeoutManager(int timeoutSec)
        {
            TimeoutSec = timeoutSec;
        }
        public bool IsTimeout()
        {
            if (LastLoginTime == null) return false;
            return (DateTime.Now - LastLoginTime.Value).TotalSeconds > TimeoutSec;
        }
        public void Login()
        {
            LastLoginTime = DateTime.Now;
        }
    }
}
