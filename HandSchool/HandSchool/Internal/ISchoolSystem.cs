using System;
using System.Collections.Generic;

namespace HandSchool.Internal
{
    interface ISchoolSystem
    {
        Dictionary<string, string> HeaderAttach { get; set; }
        Uri LoginPageUri { get; }
        Dictionary<string, string> AvaliableOperations { get; }
        bool Login(string username, string password);
    }
}
