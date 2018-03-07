using System.Collections.Generic;
using System.Net;

namespace HandSchool.JLU
{
    class Drcom : Internal.DrcomSocket
    {
        public override void SaveCredential(Dictionary<string, string> infomation)
        {
            base.SaveCredential(infomation);
            Protocol.ServerIP = IPAddress.Parse("10.100.61.3");
            Protocol.PrimaryDNS = IPAddress.Parse("10.10.10.10");
            Protocol.SecondaryDNS = IPAddress.Parse("202.98.18.3");
            Protocol.DefaultDHCP = IPAddress.Parse("0.0.0.0");
        }
    }
}
