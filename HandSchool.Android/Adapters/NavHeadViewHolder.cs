using Android.Widget;
using HandSchool.ViewModels;
using System;
using System.ComponentModel;

namespace HandSchool.Droid
{
    /// <summary>
    /// 导航栏头部的视图保持器。
    /// </summary>
    public class NavHeadViewHolder : IBindTarget, IDisposable
    {
        /// <summary>
        /// 懒加载视图保持器
        /// </summary>
        static readonly Lazy<NavHeadViewHolder> lazy =
            new Lazy<NavHeadViewHolder>(() => new NavHeadViewHolder());

        /// <summary>
        /// 保持器的实例
        /// </summary>
        public static NavHeadViewHolder Instance => lazy.Value;

        /// <summary>
        /// 欢迎消息的文本框
        /// </summary>
        [BindView(Resource.Id.nav_header_first)]
        public TextView WelcomeMessage { get; set; }

        /// <summary>
        /// 具体消息的文本框
        /// </summary>
        [BindView(Resource.Id.nav_header_second)]
        public TextView CurrentMessage { get; set; }
        
        /// <summary>
        /// 构造导航栏头部的视图保持器。
        /// </summary>
        public NavHeadViewHolder()
        {
            IndexViewModel.Instance.PropertyChanged += ModelChanged;
        }
        
        /// <summary>
        /// 视图模型属性改变监听。
        /// </summary>
        /// <param name="sender">视图模型</param>
        /// <param name="args">属性改变参数</param>
        private void ModelChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(WelcomeMessage):
                    if (WelcomeMessage != null)
                        WelcomeMessage.Text = IndexViewModel.Instance.WelcomeMessage;
                    break;
                case nameof(CurrentMessage):
                    if (CurrentMessage != null)
                        CurrentMessage.Text = IndexViewModel.Instance.CurrentMessage;
                    break;
            }
        }

        /// <summary>
        /// 目标加载后绑定数据。
        /// </summary>
        public void SolveBindings()
        {
            WelcomeMessage.Text = IndexViewModel.Instance.WelcomeMessage;
            CurrentMessage.Text = IndexViewModel.Instance.CurrentMessage;
        }

        #region IDisposable Support

        private bool disposedValue = false;
        
        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                IndexViewModel.Instance.PropertyChanged -= ModelChanged;
            }
        }

        #endregion
    }
}