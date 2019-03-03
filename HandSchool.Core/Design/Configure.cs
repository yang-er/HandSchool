using System.Threading.Tasks;
using HandSchool.Internals;

namespace HandSchool.Design
{
    public interface IConfigureProvider
    {
        /// <summary>
        /// 异步地保存设置内容。
        /// </summary>
        /// <param name="name">内容名称</param>
        /// <param name="value">内容值</param>
        Task SaveAsync(string name, string value);

        /// <summary>
        /// 异步地读取设置内容。
        /// </summary>
        /// <param name="name">内容名称</param>
        /// <returns>读取的内容</returns>
        Task<string> ReadAsync(string name);

        /// <summary>
        /// 异步地删除设置内容。
        /// </summary>
        /// <param name="name">内容名称</param>
        Task RemoveAsync(string name);
    }

    public static class ConfigureExtensions
    {
        public static Task SaveAsAsync<T>(this IConfigureProvider cp, string name, T value)
        {
            return cp.SaveAsync(name, value.Serialize());
        }

        public static async Task<T> ReadAsAsync<T>(this IConfigureProvider cp, string name)
        {
            return (await cp.ReadAsync(name)).ParseJSON<T>();
        }
    }
}
