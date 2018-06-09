using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using static HandSchool.Internal.Helper;
namespace HandSchool.Models
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class UpdateAttribute :Attribute
    {
        public UpdateAttribute(string UpdateUri, string FileName)
        {
            var wc = new AwaredWebClient(UpdateUri, Encoding.UTF8);
            var Upadtetext=wc.DownloadString("");
            var LocalText = ReadConfFile(FileName);
            if (LocalText == "")
            {
                WriteConfFile(FileName, Upadtetext);
            }
            else if (LocalText == Upadtetext)
                return;
            else
            {
                WriteConfFile(FileName, Upadtetext);
            }
        }
    }
}
