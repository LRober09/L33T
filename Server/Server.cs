using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

using L33T;
namespace Server
{
    public class Server
    {
        private int port;

        private Socket master;
        private IPAddress ip;

        private List<Connection> connections;


        public Server()
        {

        }

        public void Start(int port)
        {
            Initialize(port);

            Log("Binding master socket to local endpoint...");
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            master.Bind(endPoint);

            Log("Starting listener thread...");
            Thread listenThread = new Thread(Listen);
            listenThread.Start();
            Log("Started listening for connections on " + ip.ToString() + ":" + port + "...");
            Log("Server started successfully!");

            while (true)
            {
                Console.Write(">");
                string cmd = Console.ReadLine();

                //cmd manager
            }
        }

        private void Initialize(int port)
        {
            Log("Initializing server...");
            this.port = port;
            ip = Util.GetLocalIpAddress();
            connections = new List<Connection>();
            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            File.Delete("log.txt");
        }

        private void Listen()
        {
            while (true)
            {
                master.Listen(0);
                AcceptConnection(master.Accept());
            }
        }

        private void AcceptConnection(Socket accepted)
        {
            Log("Accepted Connection!");
            Connection connection = new Connection(accepted);
            connections.Add(connection);

            Packet packet = new Packet(PacketType.Acknowledge);

            
            Send(connection.ID, packet);
            Log("Sent acknowledgement packet!");
            packet = Recieve(connection.ID);
            Log("Recieved registration packet!");
            connection.IP = (IPAddress)packet.Data["ip"];
        }

        private Packet Recieve(string clientId)
        {
            Connection connection = connections.Find(c => c.ID == clientId);
            byte[] buffer = new byte[connection.Socket.ReceiveBufferSize];
            connection.Socket.Receive(buffer);

            return new Packet(buffer);

        }

        private void Send(string clientId, Packet packet)
        {

        }

        //Logs and prints messages if in debug mode
        private static void Log(String message)
        {
            if (Debug.DEBUG)
            {
                StreamWriter writer = new StreamWriter("log.txt", true);
                ConsoleColor orig = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - " + message);
                Console.ForegroundColor = orig;

                writer.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - " + message);
                writer.Dispose();
            }
        }
    }
}
