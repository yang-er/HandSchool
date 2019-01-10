using HandSchool.Internal.HtmlObject;
using System.Text;

namespace HandSchool.Internal
{
    public static class HtmlObjectExtensions
    {
        /// <summary>
        /// 将字符串转化为 RawHtml。
        /// </summary>
        /// <param name="value">原字符串。</param>
        /// <returns>转化对象。</returns>
        public static RawHtml ToRawHtml(this string value)
        {
            return new RawHtml { Raw = value };
        }

        /// <summary>
        /// 将字符串转化为 RawHtml。
        /// </summary>
        /// <param name="sb">原字符串。</param>
        /// <returns>转化对象。</returns>
        public static RawHtml ToRawHtml(this StringBuilder sb)
        {
            return new RawHtml { Raw = sb.ToString() };
        }

        /// <summary>
        /// 将对象外面嵌套FormGroup。
        /// </summary>
        /// <param name="obj">被嵌套内容。</param>
        /// <returns>嵌套完对象。</returns>
        public static FormGroup WrapFormGroup(this IHtmlObject obj)
        {
            return new FormGroup { Children = { obj } };
        }
    }
}
