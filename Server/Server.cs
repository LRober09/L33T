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
        public bool Running { get; set; }

        private int port;

        private Socket master;
        private IPAddress ip;

        //ONLY USE THIS FOR MANAGING CONNECTIONS! ALL USER REFERENCES SHOULD BE DONE VIA CONTROL
        private List<Connection> connections;
        private Thread listenThread;


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
            listenThread = new Thread(Listen);
            listenThread.Start();
            Log("Started listening for connections on " + ip.ToString() + ":" + port + "...");
            Log("Server started successfully!");
            Running = true;
            while (Running)
            {
                Console.Write(">");
                CommandControl(Console.ReadLine());
            }
        }

        public void Shutdown()
        {
            if (Running)
            {
                Log("Starting server shutdown...");

                Log("Closing client connections...");
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].Socket.Dispose();
                    connections[i].ClientThread.Abort();
                }

                Log("Disposing master socket...");
                master.Dispose();
                listenThread.Abort();
                ip = null;
                Running = false;
                Log("Shutdown complete!");
            }
            else throw new Exception("Server is not running!");
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
            packet.Add("id", connection.ID);

            Send(connection.ID, packet);
            packet = Recieve(connection.ID);
            connection.IP = (IPAddress)packet.Get("ip");

            connection.ClientThread = new Thread(() => ClientThread(connection.ID));
            connection.ClientThread.Start();
        }

        private void ClientThread(string clientId)
        {
            while (true)
            {
                Packet packet;
                try
                {
                    packet = Recieve(clientId);
                }
                catch (SocketException ex)
                {
                    Log("Client forcably disconnected! Removing from client pool...");
                    RemoveClient(clientId);
                    break;
                }

                PacketSwitch(packet);

            }
        }

        private void PacketSwitch(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.CheckUsername:
                    string id = packet.SenderID;
                    bool taken = Control.ContainsUser(packet);
                    if (!taken) Log("Registering new user with username: " + packet.Get("username"));
                    packet = new Packet(PacketType.CheckUsername);
                    packet.Add("taken", taken);
                    Send(id, packet);
                    break;
                default:
                    throw new UnexpectedPacketTypeException();
            }
        }

        private void CommandControl(string cmd)
        {
            switch (cmd.ToLower())
            {
                case "stop":
                    Shutdown();
                    break;

                default:
                    Print("An invalid command was entered!");
                    break;
            }
        }

        private void RemoveClient(string clientId)
        {
            Connection connection = connections.Find(c => c.ID == clientId);
            connection.Socket.Dispose();

            connections.Remove(connection);
            Control.RemoveUser(clientId);
            Log("Removed client: " + clientId);
            Log("New client pool size: " + connections.Count);
            connection.ClientThread.Abort();
        }

        private Socket GetSocket(string clientId)
        {
            return connections.Find(c => c.ID == clientId).Socket;
        }

        private Packet Recieve(string clientId)
        {
            Socket socket = GetSocket(clientId);
            byte[] buffer = new byte[socket.ReceiveBufferSize];
            socket.Receive(buffer);
            return new Packet(buffer);

        }

        private void Send(string clientId, Packet packet)
        {
            GetSocket(clientId).Send(packet.Serialize());
        }

        //Logs and prints messages if in debug mode
        private static void Log(string message)
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

        private static void Print(string message)
        {
            ConsoleColor orig = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = orig;
        }
    }
}
