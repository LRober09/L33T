using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace L33T
{
    public static class Debug
    {
        public const bool DEBUG = true;
        public const bool CLIENT_DEBUG = true;

        public static void WriteAddressInfo()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine("Hostname: " + host.HostName);
            foreach (IPAddress ip in host.AddressList)
            {
                Console.WriteLine("\nAddress family: " + ip.AddressFamily);
                Console.WriteLine("Address: " + ip.ToString());
            }
        }
    }
}
