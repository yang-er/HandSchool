using Microcharts;
using SkiaSharp.Views.UWP;

namespace HandSchool.Views
{
    /// <summary>
    /// 展示图表的对话框。
    /// </summary>
    public sealed partial class ChartDialog : ViewDialog
    {
        /// <summary>
        /// 展示的图表对象。
        /// </summary>
        public Chart Chart { get; }

        /// <summary>
        /// 创建一个用于查看图表的对话框。
        /// </summary>
        /// <param name="charts">图表对象</param>
        /// <param name="title">对话框标题</param>
        public ChartDialog(Chart charts, string title = null)
        {
            InitializeComponent();
            Title = title ?? "查看图表";
            Chart = charts;
        }

        /// <summary>
        /// Canvas绘制图表。
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">绘制表面的参数</param>
        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}
