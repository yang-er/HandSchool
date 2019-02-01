using System;

namespace HandSchool.Droid
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BindViewAttribute : Attribute
    {
        public int ResourceId { get; }

        public BindViewAttribute(int id)
        {
            ResourceId = id;
        }
    }
}