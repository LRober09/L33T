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
    class Server
    {
        private static int port;

        private static Socket master;
        private static IPAddress ip;

        private static List<Connection> connections;


        static void Main(string[] args)
        {
            DM("Starting server...");

            connections = new List<Connection>();
            File.Delete("log.txt");

            DM("Initializing socket...");
            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ip = Util.GetLocalIpAddress();


            port = Util.PORT;
            IPEndPoint ipend = new IPEndPoint(ip, port);
            DM("Binding master socket...");
            master.Bind(ipend);
            DM("Server successfully started on port " + port);

            Listen();
        }

        private static void Listen()
        {
            DM("Listening for connections at " + ip.ToString() + ":" + port + "...");

            while (true)
            {
                master.Listen(0);
                AcceptConnection(master.Accept());
            }
        }

        private static void AcceptConnection(Socket accepted)
        {
            //Mostly bookkeeping stuff
            Connection newConnection = new Connection(accepted);

            DM("Accepted connection! Sending response packet...");

            newConnection.ClientThread = new Thread(() => ConnectionManager(newConnection));

            DM("Assigned client to ID: " + newConnection.ID);

            Packet p = new Packet(PacketType.ConnectionResponse);
            p.Data.Add("id", newConnection.ID);
            newConnection.Socket.Send(p.Serialize());

            byte[] configBuffer = new byte[accepted.ReceiveBufferSize];
            newConnection.Socket.Receive(configBuffer);

            p = new Packet(configBuffer);
            DM("Recieved config response from client! IP Address: " + p.Data["ip"].ToString());
            newConnection.IP = (IPAddress)p.Data["ip"];
            connections.Add(newConnection);
            newConnection.ClientThread.Start();
        }

        public static void ConnectionManager(Connection connection)
        {
            DM(connection.ID + " has successfully connected!");

            byte[] buffer;
            while (true)
            {
                if (!connection.Socket.Connected)
                {
                    DM(connection.ID + " has disconnected!");
                    break;
                }

                buffer = new byte[connection.Socket.ReceiveBufferSize];

                try
                {
                    connection.Socket.Receive(buffer);
                }
                catch (SocketException ex)
                {
                    DM(connection.ID + " has disconnected! Removing from client list...");
                    connection.Socket.Dispose();
                    connections.Remove(connection);
                    DM("New client pool size: " + connections.Count);
                    connection.ClientThread.Abort();
                }
            }

        }

        //Logs and prints messages if in debug mode
        private static void DM(String message)
        {
            if (Debug.DEBUG)
            {
                StreamWriter writer = new StreamWriter("log.txt", true);
                ConsoleColor orig = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ForegroundColor = orig;

                writer.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] - " + message);
                writer.Dispose();
            }
        }
    }
}
