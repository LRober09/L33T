using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using L33T;

namespace Client
{
    public class ClientRun
    {
        public static void Main(string[] args)
        {
            Client client = new Client();
            client.Initialize();

            Console.Write("Enter server IP or leave blank for local: ");
            string input = Console.ReadLine();
            IPAddress ip;
            if(input != string.Empty)
            {
                ip = IPAddress.Parse(input);
            } else
            {
                ip = Util.GetLocalIpAddress();
            }

            Console.Write("Enter port (default 3000): ");
            input = Console.ReadLine();
            int port;
            if(input != string.Empty)
            {
                port = Convert.ToInt16(input);
            } else
            {
                port = 3000;
            }
            Console.WriteLine("Connecting...");
            client.Connect(ip, port);
        }
    }
}
