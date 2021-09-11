using HandSchool.Controls;
using HandSchool.Internal;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool
{
    public static class TextAtomScales
    {
        public static readonly double Normal = 0.95;
        public static readonly double Small = 0.95 * 0.90;
        public static readonly double Large = 0.95 * 1.05;
        public static readonly double SuperLarge = 0.95 * 1.1;
    }
    public static class Extends
    {
        public async static Task TappedAnimation(this VisualElement item, Func<Task> doing)
        {
            if (item == null) return;
            await item.ScaleTo(TextAtomScales.Small, 200);
            if(doing !=null)await doing();
            await item.ScaleTo(TextAtomScales.Large, 200);
            await item.ScaleTo(TextAtomScales.Normal, 150);
        }
        public async static Task LongPressAnimation(this VisualElement item, Func<Task> doing)
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
