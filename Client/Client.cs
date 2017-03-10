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
    public class Client
    {

        public string ID { get; set; }

        private Socket local;
        private IPAddress ip;


        public void Initialize()
        {
            ip = Util.GetLocalIpAddress();
            local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(IPAddress address, int port)
        {
            if (local == null) throw new ClientNotInitializedException();

            IPEndPoint connectionEndPoint = new IPEndPoint(address, port);
            local.Connect(connectionEndPoint);
            Console.WriteLine("Contacted server! Registering...");

            //Register with server
            Packet packet = Recieve();
            Console.WriteLine("Recieved acknowledgement packet!");
            if (packet.Type == PacketType.Acknowledge)
            {
                this.ID = packet.Get("id").ToString();
                Console.WriteLine("Set ID to " + ID);
                packet = new Packet(PacketType.Register, ID);
                packet.Add("ip", ip);
                Send(packet);

                Console.WriteLine("Send IP packet");
            }
            else throw new UnexpectedPacketTypeException();

            Console.WriteLine("Successfully connected! Press any key to start...");
            Console.ReadKey();
            Join();



        }

        private void Join()
        {
            Console.Clear();
        A: Console.Write("Enter a username: ");
            string username = Console.ReadLine();
            Packet packet = new Packet(PacketType.CheckUsername);
            packet.Add("username", username);
            Send(packet);
            packet = Recieve();
            if ((bool)packet.Get("taken"))
            {
                Console.WriteLine("That username is already in use!");
                goto A;
            }
            else
            {
                packet = new Packet(PacketType.Register);
                
                Console.WriteLine("Success!");
                Console.ReadLine();
            }
        }

        private void Send(Packet p)
        {
            //Ensure that packets have a sender ID when coming from the client
            if (p.SenderID == "server")
            {
                p.SenderID = ID;
            }


            local.Send(p.Serialize());
        }

        private Packet Recieve()
        {
            byte[] buffer = new byte[local.ReceiveBufferSize];
            local.Receive(buffer);
            return new Packet(buffer);
        }
    }
}
