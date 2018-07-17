using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandSchool.Internal
{
    /// <summary>
    /// 实现了 <see cref="INotifyPropertyChanged" /> 的基类，提供数据绑定的双向响应。
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="backingStore">属性储存项</param>
        /// <param name="value">属性值</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="onChanged">改变时触发</param>
        /// <returns>是否改变了原值</returns>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        
        /// <summary>
        /// 属性改变时触发
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
