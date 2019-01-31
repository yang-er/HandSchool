using Android.Content;

namespace HandSchool.Droid
{
    public static class DroidExtensions
    {
        /// <summary>
        /// 将dp值转换为px值
        /// </summary>
        /// <param name="dpValue">dp值</param>
        /// <returns>px值</returns>
        public static int Dip2Px(this Context context, float dpValue)
        {
            return (int)(dpValue * context.Resources.DisplayMetrics.Density + 0.5f);
        }

        /// <summary>
        /// 将px值转换为dp值
        /// </summary>
        /// <param name="pxValue">px值</param>
        /// <returns>dp值</returns>
        public static float Px2Dip(this Context context, int pxValue)
        {
            return pxValue / context.Resources.DisplayMetrics.Density;
        }
    }
}