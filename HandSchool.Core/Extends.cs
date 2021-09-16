using HandSchool.Controls;
using HandSchool.Internal;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool
{
    public static class SimplifiedNames
    {

        static Dictionary<string, int> chineseNums = null;
        static void InitCnNums()
        {
            chineseNums = new Dictionary<string, int>();
            chineseNums.Add("零", 0);
            chineseNums.Add("一", 1);
            chineseNums.Add("二", 2);
            chineseNums.Add("三", 3);
            chineseNums.Add("四", 4);
            chineseNums.Add("五", 5);
            chineseNums.Add("六", 6);
            chineseNums.Add("七", 7);
            chineseNums.Add("八", 8);
            chineseNums.Add("九", 9);
            chineseNums.Add("十", 10);
            chineseNums.Add("十一", 11);
            chineseNums.Add("十二", 12);
            chineseNums.Add("十三", 13);
            chineseNums.Add("十四", 14);
            chineseNums.Add("十五", 15);
        }
        static string ChineseToNum(string str)
        {
            if (chineseNums == null) InitCnNums();
            if (chineseNums.ContainsKey(str)) return chineseNums[str].ToString();
            return str;
        }
        static string SimplifyRoomName(string roomName)
        {
            var ruler = new Regex("[A-Za-z0-9]区第.+阶梯");
            var room = ruler.Match(roomName);
            if (room.Length != 0)
            {
                var str = room.Value;
                char area = str[str.IndexOf("区") - 1];
                var index1 = str.IndexOf("第");
                var index2 = str.IndexOf("阶");
                var area2 = str.Substring(index1 + 1, index2 - index1 - 1);
                var str2 = area + ChineseToNum(area2);
                return roomName.Replace(str, str2);
            }

            ruler = new Regex("第.+阶梯");
            room = ruler.Match(roomName);
            if (room.Length != 0)
            {
                var str = room.Value;
                var index1 = str.IndexOf("第");
                var index2 = str.IndexOf("阶");
                var area2 = str.Substring(index1 + 1, index2 - index1 - 1);
                var str2 = ChineseToNum(area2) + "阶";
                return roomName.Replace(str, str2);
            }

            return roomName;
        }
        public static string SimplifyName(string str)
        {
            var res = SimplifyRoomName(str);
            return res.Replace("教学楼", "楼").Replace('-', '\n');
        }
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
            return Color.FromRgb(GetColorNum(color.R, rate), GetColorNum(color.G, rate), GetColorNum(color.B, rate));
        }
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
