using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandSchool.Internals
{
    /// <summary>
    /// 实现了 <see cref="INotifyPropertyChanged" /> 的基类，提供数据绑定的双向响应。
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// 当属性改变时触发事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 设置保存属性的值，并检查是否改变。
        /// </summary>
        /// <typeparam name="T">保存的属性类型。</typeparam>
        /// <param name="backingStore">属性后台储存项的引用。</param>
        /// <param name="value">改变的属性新值。</param>
        /// <param name="propertyName">改变的属性的名称。推荐使用 <see cref="nameof"/> 来指代。</param>
        /// <param name="onChanged">在改变时触发 <see cref="Action"/> 完成自定义的属性改变事件。</param>
        /// <returns>是否改变了原值</returns>
        [DebuggerStepThrough]
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
        /// 属性改变时简化地触发事件。
        /// </summary>
        /// <param name="propertyName">改变了值的属性的名称。推荐使用 <see cref="nameof"/> 来指代。</param>
        [DebuggerStepThrough]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 检测绑定数目，以调试内存泄漏。
        /// </summary>
        /// <returns>绑定数目</returns>
        protected int GetEventAttached()
        {
            return PropertyChanged?.GetInvocationList().Length ?? 0;
        }
    }
}
