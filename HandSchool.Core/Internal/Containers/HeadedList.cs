using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HandSchool.Internals
{
    public class HeadedObservableCollection<T> : ObservableCollection<T>
    {
        public HeadedObservableCollection(string header)
        {
            GroupHeader = header;
        }

        /// <summary>
        /// 组标题
        /// </summary>
        public string GroupHeader { get; set; }
    }

    public class HeadedList<T> : List<T>
    {
        public HeadedList(string header)
        {
            GroupHeader = header;
        }

        public HeadedList(string header, IEnumerable<T> src) : base(src)
        {
            GroupHeader = header;
        }

        /// <summary>
        /// 组标题
        /// </summary>
        public string GroupHeader { get; set; }
    }
}
