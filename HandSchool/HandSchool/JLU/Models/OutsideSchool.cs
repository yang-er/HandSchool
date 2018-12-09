using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    partial class UIMS
    {
        class OutsideSchoolStrategy : ISideSchoolStrategy
        {
            UIMS UIMS { get; }

            public OutsideSchoolStrategy(UIMS handle)
            {
                UIMS = handle;
            }

            public string TimeoutUrl => throw new NotImplementedException();

            public string WelcomeMessage => throw new NotImplementedException();

            public string CurrentMessage => throw new NotImplementedException();

            public string FormatArguments(string input)
            {
                throw new NotImplementedException();
            }

            public Task<bool> LoginSide()
            {
                throw new NotImplementedException();
            }

            public void OnLoad()
            {
                throw new NotImplementedException();
            }
        }
    }
}
