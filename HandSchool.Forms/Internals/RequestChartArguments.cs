using Microcharts;
using System.Threading.Tasks;

namespace HandSchool.Forms
{
    /// <summary>
    /// 请求展示图表内容的参数列表。
    /// </summary>
    public class RequestChartArguments
    {
        /// <summary>
        /// 展示的图表
        /// </summary>
        public Chart Chart { get; }

        /// <summary>
        /// 对话框标题
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 对话框关闭字样
        /// </summary>
        public string Close { get; }

        /// <summary>
        /// 结束的任务
        /// </summary>
        public Task ReturnTask { get; }

        /// <summary>
        /// 创建请求展示图表内容的参数列表。
        /// </summary>
        /// <param name="chart">展示的图表</param>
        /// <param name="title">对话框标题</param>
        /// <param name="close">对话框关闭字样</param>
        public RequestChartArguments(Chart chart, string title, string close)
        {
            Chart = chart;
            Title = title;
            Close = close;
            ReturnTask = new Task(() => { });
        }
    }
}
