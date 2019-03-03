using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandSchool.Models;

namespace HandSchool.Services
{
    /// <summary>
    /// 信息热点获取服务
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IFeedEntrance
    {
        /// <summary>
        /// 获取第n页新闻。
        /// </summary>
        /// <param name="n">页号</param>
        /// <exception cref="ServiceException" />
        /// <returns>下次查询页号与此次获取到的内容</returns>
        Task<Tuple<int, IEnumerable<FeedItem>>> FetchAsync(int n);

        /// <summary>
        /// 从缓存中读取数据。
        /// </summary>
        /// <returns>缓存的内容，若没有则为null</returns>
        Task<IEnumerable<FeedItem>> FromCacheAsync();
    }
}