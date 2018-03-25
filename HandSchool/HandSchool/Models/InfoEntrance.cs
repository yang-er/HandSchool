using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace HandSchool
{
    public interface IInfoEntrance : ISystemEntrance
    {
        string Description { get; set; }
    }
}
