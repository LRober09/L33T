using L33TPackets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L33TServer
{
    class L33TServer
    {
        #region PrimaryDeclarations/Initializations
        static string LocalIP4;
        public static Socket master;
        static List<Client> clientList;
        static List<Thread> _clientManager;
        static bool running = true;
        static Thread connectionListener;

        static void Initialize()
        {
            clientList = new List<Client>();
            _clientManager = new List<Thread>();
            LocalIP4 = Packet.GetLocalIP4().ToString();
            connectionListener = new Thread(ClientListener);
        }

        #endregion



        static void Main(string[] args)
        {
            //Temporary interface:
            Console.Title = "L33T Server";
            MainMenu();

        }

        static void MainMenu()
        {
            string input = "";
            while (input != "1" && input != "'1'")
            {
                Format();
                Console.WriteLine("~~~~~| L33T H4X0R |~~~~~");
                Console.WriteLine("'1' Start Server on this IP Address");
                Console.WriteLine("'E' Exit");

                input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case "1":
                    case "'1'":
                        StartServer();
                        break;

                    case "e":
                    case "'e'":
                        Environment.Exit(0);
                        break;

                    default:
                        ShowError("Invalid input!");
                        break;

                }
            }
        }



        static void StartServer()
        {
            Initialize();
            Format();

            Console.WriteLine("Starting server on: " + LocalIP4);
            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(LocalIP4), 2700);
            master.Bind(ip);

            connectionListener.Start();
            Console.WriteLine("Server successfully started!");
            Thread inputThread = new Thread(InputThread);
            Thread.Sleep(1000);
            inputThread.Start();
            Console.Clear();
            Console.WriteLine("Listening for clients...");
            Console.WriteLine("(Host IP: " + LocalIP4 + ")");
        }


        static void InputThread()
        {

            for (; ; )
            {
                string input = Console.ReadLine().ToLower();


                switch (input)
                {
                    case "stop":
                        #region Cleanly
                        //running = false;
                        //for (int i = 0; i < _clientManager.Count; i++)
                        //{
                        //    _clientManager[i].Abort();
                        //    Thread.Sleep(10);
                        //    _clientManager.RemoveAt(i);
                        //}
                        //for (int i = 0; i < clientList.Count; i++)
                        //{
                        //    clientList[i].sck.Close();
                        //    clientList[i].sck.Dispose();
                        //    clientList[i].clientThread.Abort();
                        //    clientList.RemoveAt(i);
                        //}
                        //master.Close();
                        //master.Dispose();
                        #endregion
                        Environment.Exit(0);
                        //Ugly way
                        break;

                    case "kick":
                        string name = input.Split(' ')[1];
                        Client c;
                        foreach (Client client in clientList)
                        {
                            if (client.name == name)
                            {
                                client.sck.Close();
                                clientList.Remove(client);
                            }
                        }
                        break;

                }
            }
        }
        static void ClientListener()
        {
            clientList.Clear();
            while (running)
            {
                master.Listen(1);

                Socket accepted = master.Accept();
                Client client = new Client(accepted);

                Packet p = new Packet(PacketType.RegRequest);
                accepted.Send(p.ToBytes());


                clientList.Add(client);
                //_clientManager.Add(new Thread(DataManager_IN));
                //_clientManager[clientCount].Start(client);
                client.clientThread = new Thread(DataManager_IN);
                client.clientThread.Start(client);


                ConnectionManagerMessage("Client connected: " + client.ip.ToString());
                foreach (Client c in clientList)
                {
                    //DebugMessage(c.ip.ToString());
                }
            }

        }

        static void DataManager_IN(object INclient)
        {
            Client client = (Client)INclient;
            Socket sck = client.sck;
            //DebugMessage("Started datamanger_IN for " + client.ip);
            byte[] Buffer;

            try
            {

                while (true)
                {
                    Buffer = new byte[sck.SendBufferSize];
                    int bytesRead = sck.Receive(Buffer);

                    if (bytesRead > 0)
                    {
                        //DebugMessage("read bytes of more than 1");
                        Packet packet = new Packet(Buffer);
                        PacketMessage("Recieved a packet, sending to manager...");
                        ServerPacketManager(packet);
                    }

                }
            }
            catch (SocketException)
            {
                for (int i = 0; i < clientList.Count; i++)
                {
                    if (clientList[i].name == client.name)
                    {
                        clientList.RemoveAt(i);
                    }
                }

                ConnectionManagerMessage("Client disconnected: " + client.ip + ", " + client.name);
                client.clientThread.Abort();
            }
        }

        static void ServerPacketManager(Packet packet)
        {
            switch (packet.type)
            {
                case PacketType.Broadcast:
                    //DebugMessage("Recieved a broadcast packet");
                    Broadcast(packet);
                    break;

                case PacketType.FCheck:
                    Forward(packet);
                    break;

                case PacketType.FCheckReturn:
                    Forward(packet);
                    break;

                case PacketType.FGet:
                    Forward(packet);
                    break;
                case PacketType.FGetReturn:
                case PacketType.FGetDenied:
                    Forward(packet);
                    break;

                case PacketType.GetFrozen:
                case PacketType.Flood:
                case PacketType.GetFrozenReturn:
                    Forward(packet);

                    break;

                case PacketType.Grab:
                case PacketType.GrabDenied:
                case PacketType.GrabReturn:
                    Forward(packet);
                    break;

                case PacketType.RegRequest:
                    RegRequest(packet);
                    break;

                case PacketType.ScanReq:
                    Scan(packet);
                    break;

                case PacketType.UpgradeUpdate:
                    UpgradeUpdate(packet);
                    break;

                case PacketType.Virus:
                    Forward(packet);
                    break;

                default:
                    ShowErrorNoSleep("Recieved a packet with an unknown type!");
                    break;
            }
        }

        static Client GetClient(string ip)
        {
            Client client = null;
            foreach (Client c in clientList)
            {
                if (c.ip.ToString() == ip)
                    client = c;
            }


            return client;
        }

        #region PacketManagers
        static void Broadcast(Packet p)
        {
            foreach (Client c in clientList)
            {
                c.sck.Send(p.ToBytes());
            }
        }

        static void FCheckForward(Packet p)
        {
            Client client = GetClient(p.data[0]);
            client.sck.Send(p.ToBytes());

            //foreach (Client c in clientList)
            //    if (c.ip.ToString() == p.data[0])
            //    {
            //        c.sck.Send(p.ToBytes());
            //    }
        }
        static void FCheckForwardReturn(Packet p)
        {

            Client client = GetClient(p.data[1]);
            client.sck.Send(p.ToBytes());

            //foreach (Client c in clientList)
            //    if (c.ip.ToString() == p.data[1])
            //    {
            //        c.sck.Send(p.ToBytes());
            //    }
        }
        static void FGetForward(Packet p)
        {
            Client client = GetClient(p.data[0]);
            client.sck.Send(p.ToBytes());
        }
        static void FGetForwardReturn(Packet p)
        {
            Client client = GetClient(p.data[1]);
            client.sck.Send(p.ToBytes());
        }

        static void FloodFreezeForward(Packet p)
        {
            GetClient(p.data[0]).sck.Send(p.ToBytes());
        }

        static void GrabForward(Packet p)
        {
            Client client = GetClient(p.data[0]);
            client.sck.Send(p.ToBytes());
        }

        static void Forward(Packet p)
        {
            GetClient(p.data[0]).sck.Send(p.ToBytes());
        }

        static void Scan(Packet p)
        {

            #region ClientAssignments
            Client sender = GetClient(p.senderIP.ToString());
            //foreach (Client c in clientList)
            //{
            //    if (c.ip.ToString() == p.senderIP.ToString())
            //        sender = c;
            //}
            #endregion

            Packet rp = new Packet(PacketType.ScanReq);
            foreach (Client c in clientList)
            {
                if (c.upgrades.Contains("firewall3"))
                {
                    if (sender.upgrades.Contains("scanner2"))
                    {
                        rp.data.Add(c.ip.ToString() + " (" + c.name + ")");
                    }
                    else rp.data.Add("***.***.**.*");
                }
                else if (c.upgrades.Contains("firewall2"))
                {
                    if (sender.upgrades.Contains("scanner2"))
                    {
                        rp.data.Add(c.ip.ToString() + " (" + c.name + ")");
                    }
                    else if (sender.upgrades.Contains("scanner1"))
                    {
                        rp.data.Add(c.ip.ToString());
                    }
                    else rp.data.Add("***.***.**.*");
                }
                else
                {
                    if (sender.upgrades.Contains("scanner2"))
                        rp.data.Add(c.ip.ToString() + " (" + c.name + ")");
                    else
                        rp.data.Add(c.ip.ToString());
                    DebugMessage("Cycling through ip addresses, added: " + c.ip.ToString());
                }
            }


            Client client = GetClient(p.senderIP.ToString());
            client.sck.Send(rp.ToBytes());
            //foreach (Client c in clientList)
            //{
            //    if (c.ip.ToString() == p.senderIP.ToString())
            //        c.sck.Send(rp.ToBytes());
            //}
        } //test scan to make sure upgrade works

        static void RegRequest(Packet p)
        {
            clientList[clientList.Count - 1].name = p.data[0];
            clientList[clientList.Count - 1].ip = p.senderIP;
            clientList[clientList.Count - 1].registered = true;
        }

        static void UpgradeUpdate(Packet p)
        {

            Client client = GetClient(p.senderIP.ToString());
            foreach (string s in p.data)
                client.upgrades.Add(s);
            DebugMessage("Added upgrades to " + client.name + "'s profile");


            //foreach (Client c in clientList)
            //{
            //    if (c.ip.ToString() == p.senderIP.ToString())
            //    {
            //        foreach (string s in p.data)
            //            c.upgrades.Add(s);
            //        DebugMessage("Added upgrades to " + c.name + "'s profile");
            //    }
            //}
        }

        static void ForwardVirus(Packet p)
        {
            Client client = GetClient(p.data[0]);
            client.sck.Send(p.ToBytes());
            DebugMessage("Sent a virus packet to " + client.ip.ToString());

            //foreach (Client c in clientList)
            //{
            //    if (c.ip.ToString() == p.data[0])
            //    {
            //        c.sck.Send(p.ToBytes());
            //        DebugMessage("Sent a virus packet to " + c.ip.ToString());
            //    }
            //}
        }

        #endregion






        #region ConsoleStuff
        static void Format()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
        }
        static void ShowError(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
            Thread.Sleep(1000);
        }
        static void ShowErrorNoSleep(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
            Thread.Sleep(1000);
        }
        static void ConnectionManagerMessage(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
        static void PacketMessage(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
        static void DebugMessage(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
        #endregion
    }



}
 