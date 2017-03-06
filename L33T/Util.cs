using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace L33T
{
    public static class Util
    {
        public const int PORT = 3000;

        public static IPAddress GetLocalIpAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
        }
    }
}
