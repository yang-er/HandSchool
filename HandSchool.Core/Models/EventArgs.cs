using System;

namespace HandSchool.Models
{
    public class CollectionItemTappedEventArgs : EventArgs
    {
        public readonly struct IndexPath
        {
            public readonly int GroupIndex;
            public readonly int IndexInGroup;

            public IndexPath(int groupIndex, int indexInGroup)
            {
                GroupIndex = groupIndex;
                IndexInGroup = indexInGroup;
            }
        }
        public IndexPath Path { get; set; }
        public object Item { get; set; }
        public object Group { get; set; }
    }
    
    public class ClassLoadedEventArgs : EventArgs
    {
        public int ResIndex = -1;
        public System.Collections.Generic.IList<CurriculumItem> Classes;
    }
    
    public class IsBusyEventArgs : EventArgs
    {
        public bool IsBusy { get; set; }
    }
}