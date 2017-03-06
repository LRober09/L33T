using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using L33T;

namespace Client
{
    class Client
    {
        private static Socket local;
        private static IPAddress ip;
        private static string id;

        static void Main(string[] args)
        {
            local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ip = Util.GetLocalIpAddress();
            Connect();
            
        }

        static void Connect()
        {
            IPAddress connectionIp;
            if (Debug.CLIENT_DEBUG)
            {
                connectionIp = IPAddress.Parse("10.0.1.10");
                Console.WriteLine("Press any key to connect to server...");
                Console.ReadKey();
            }

            local.Connect(connectionIp, Util.PORT);

            byte[] buffer = new byte[local.ReceiveBufferSize];
            local.Receive(buffer);

            Packet p = new Packet(buffer);

            if(p.Type == PacketType.ConnectionResponse)
            {
                id = p.Data["id"].ToString();
                Console.WriteLine(id);
                p = new Packet(PacketType.ConnectionConfirm);
                p.Data.Add("ip", ip);
                local.Send(p.Serialize());
            }

            Console.WriteLine(p.Type);
            Console.ReadLine();
        }
    }
}
