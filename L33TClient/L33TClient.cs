using L33TEngine;
using L33TPackets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace L33TClient
{
    class L33TClient
    {
        public static string name;
        public static string upassword;
        public static IPAddress ipaddress;
        public static Socket master;

        // public static List<string> upgrades;
        //  public static int bytes;


        static void Main(string[] args)
        {
            Console.Title = "L33T Client";
            //upgrades = new List<string>();
            Engine.CheckLocalFiles();
            //bytes = 0;
            ipaddress = Packet.GetLocalIP4();
            MainMenu();

        }

        static void MainMenu()
        {
            while (true)
            {
            A: Format();
                Console.WriteLine("~~~~~| L33T H4X0R Client |~~~~~");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                if (!CheckUserExists())
                {
                    Console.WriteLine("Current user: No data found! Select 'Join a game' to create a profile...\n");
                }
                else
                {
                    Console.WriteLine("Current user: " + name + "\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                //Console.WriteLine();

                Console.WriteLine("'1' Join a game");
                Console.ForegroundColor = ConsoleColor.Green;

                if (!CheckUserExists())
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("'2' Change password");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("'3' Tutorial");

                Console.WriteLine("'E' Exit");
                string input = Console.ReadLine().ToLower();

                switch (input)
                {
                    case "1":
                    case "'1'":
                        if (!CheckUserExists())
                            CreateUserInterface();
                        JoinServer();
                        break;

                    case "2":
                    case "'2'":
                        if (!CheckUserExists())
                            CreateUserInterface();
                        else
                            ChangePassword();
                        break;

                    case "3":
                    case "'3'":
                        Tutorial.Play();
                        break;

                    case "e":
                    case "'e'":
                        Environment.Exit(0);
                        break;

                    default:
                        ShowError("Invalid input!");
                        goto A;
                }
            }

        }

        static void JoinServer()
        {
        A: Format();
            Console.Write("Enter host IP: ");
            string input = Console.ReadLine();

            #region CheckIP
            try
            {
                IPAddress.Parse(input);
            }
            catch
            {
                ShowError("Input was not a valid IP address!");
                goto A;
            }
            #endregion
            Console.WriteLine("Connecting to server...");

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(input), 2700);


            try
            {
                master.Connect(ip);
            }
            catch
            {
                ShowError("Coudn't connect to host!");
                goto A;
            }

            Thread datamanger = new Thread(DataManager_IN);
            datamanger.Start(master);
            Console.WriteLine("(Local IP is: " + ip);
            Thread.Sleep(1000);
            Engine.master = master;
            Engine.name = name;
            Engine.password = upassword;
            Engine.is_flooded = false;
            Engine.frozen = false;
            Engine.DispGameInterface(name);


        }
        static bool CheckUserExists()
        {
            if (File.Exists("user.data"))
            {
                UserData data = UserData.Load();
                name = data.username;
                upassword = data.password;
                return true;
            }
            else return false;
        }
        static void CreateUserInterface()
        {
            Format();
            Console.WriteLine("~~~~~| Create User |~~~~~");
            Console.Write("Enter a username: ");
            string username = Console.ReadLine().ToLower();
        A: Console.Write("Enter a password (6 chars max): ");
            string password = Console.ReadLine().ToLower();

            if (password.Length > 6 || password.Contains(' '))
            {
                ShowError("Password must be no more than 6 characters and cannot contain a space!");
                goto A;
            }

            UserData data = new UserData(username, password);
            name = username;
            upassword = password;
            data.Save();
        }
        static void ChangePassword()
        {
        A: Format();
            Console.Write("Enter a new password (6 chars max): ");
            string pass = Console.ReadLine();
            if (pass.Length > 6 || pass.Contains(' '))
            {
                ShowError("Password must be no more than 6 characters and cannot contain a space!");
                goto A;
            }
            UserData data = new UserData(name, pass);
            data.Save();
        }

        static void DataManager_IN(object socket)
        {
            Socket sck = (Socket)socket;
            byte[] Buffer;
            int bytesRead;

            try
            {
                while (true)
                {
                    Buffer = new byte[sck.SendBufferSize];
                    bytesRead = sck.Receive(Buffer);

                    if (bytesRead > 0)
                    {
                        Packet p = new Packet(Buffer);
                        ClientPacketManager(p);
                    }



                }
            }
            catch (SocketException)
            {
                Format();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The server disconnected!");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Exiting in 5 seconds...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            catch
            {
                Format();
                Console.WriteLine("An unknown error occurred!");
                Console.ReadLine();
                Environment.Exit(2);
            }
        }




        static void ClientPacketManager(Packet packet)
        {
            switch (packet.type)
            {

                case PacketType.Broadcast:
                    Broadcast(packet);
                    break;

                case PacketType.FCheck:
                    FCheck(packet);
                    break;

                case PacketType.FCheckReturn:
                    Engine.returner = packet.data[1];
                    break;

                case PacketType.FGet:
                    FGet(packet);
                    break;
                case PacketType.FGetReturn:
                    FGetReturn(packet);
                    break;
                case PacketType.FGetDenied:
                    Console.WriteLine("Target system actively refused connection!");
                    break;

                case PacketType.GetFrozen:
                    GetFrozen(packet);
                    break;
                case PacketType.GetFrozenReturn:
                    GetFrozenReturn(packet);
                    break;
                case PacketType.Flood:
                    Flood();
                    break;

                case PacketType.Grab:
                    Grab(packet);
                    break;
                case PacketType.GrabReturn:
                case PacketType.GrabDenied:
                    GrabReturn(packet);
                    break;


                case PacketType.RegRequest:
                    //Console.WriteLine("Got a reg request");
                    ReturnRegInfo();
                    break;


                case PacketType.ScanReq:
                    ScanResults(packet);
                    break;

                case PacketType.Virus:
                    Engine.InstallVirus((Virus)packet.packetObject);
                    break;

            }

        }

        #region ClientPacketManagers

        static void Broadcast(Packet p)
        {
            if (!Engine.muteList.Contains(p.data[0]) && !Engine.frozen)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(p.data[0] + ": ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(p.data[1]);
                Engine.PromptOut();
            }
        }

        static void FCheck(Packet p)
        {
            int fwtier = 0;
            bool blocked = false;
            foreach (Upgrade u in Engine.upgradeList)
                if (u.name == "firewall3")
                {
                    fwtier = 3;
                    break;
                }
                else if (u.name == "firewall2")
                {
                    fwtier = 2;
                    break;
                }
                else if (u.name == "firewall1")
                {
                    fwtier = 1;
                    break;
                }

            Random r = new Random();
            int chance = r.Next(1, 11);

            switch (fwtier)
            {
                case 3:
                    if (chance <= 6)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }
                    break;
                case 2:
                    if (chance <= 4)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;

                    }
                    break;
                case 1:
                    if (chance <= 2)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }

                    break;
            }

            string returner = "";
            foreach (Virus v in Engine.installedViruses)
            {
                if (v.type == VirusType.Inhibitor)
                {

                    returner = "true";
                }
            }
            if (returner == "")
                returner = "false";

        A: if (blocked || (Engine.frozen && !Engine.is_flooded))
                returner = "blocked";
            Packet packet = new Packet(PacketType.FCheckReturn);
            packet.data.Add(p.senderIP.ToString());
            packet.data.Add(returner);
            

            master.Send(packet.ToBytes());



        }

        static void FGet(Packet p)
        {
            #region Firewall Stuff
            int fwtier = 0;
            bool blocked = false;
            foreach (Upgrade u in Engine.upgradeList)
                if (u.name == "firewall3")
                {
                    fwtier = 3;
                    break;
                }
                else if (u.name == "firewall2")
                {
                    fwtier = 2;
                    break;
                }
                else if (u.name == "firewall1")
                {
                    fwtier = 1;
                    break;
                }

            Random r = new Random();
            int chance = r.Next(1, 11);

            switch (fwtier)
            {
                case 3:
                    if (chance <= 6)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }
                    break;
                case 2:
                    if (chance <= 4)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;

                    }
                    break;
                case 1:
                    if (chance <= 2)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }

                    break;
            }
            #endregion

            if (blocked)
            {
                Packet packet = new Packet(PacketType.FGetDenied);
                packet.data.Add(p.senderIP.ToString());
                

                master.Send(packet.ToBytes());
            }
            else
            {
                Packet packet = new Packet(PacketType.FGetReturn);
                
                packet.data.Add(p.senderIP.ToString());
                packet.data.Add(Engine.password);
                packet.data.Add(p.data[1]);

                master.Send(packet.ToBytes());
            }
        }

        static void FGetReturn(Packet p)
        {
            PasswordHash ph = new PasswordHash(p.data[1], p.data[2]);
            ph.Save();
            Console.WriteLine("Saved password hash in files as " + p.data[2] + ".hsh");
            Engine.PromptOut();
        }

        static void Flood()
        {
            Engine.Flood();
        }
        static void GetFrozen(Packet p)
        {
            Packet packet = new Packet(PacketType.GetFrozenReturn);
            packet.data.Add(p.senderIP.ToString());
            if (Engine.frozen)
                packet.data.Add("true");
            else
                packet.data.Add("false");

            Engine.master.Send(packet.ToBytes());
        }
        static void GetFrozenReturn(Packet p)
        {
            Engine.returner = p.data[1];
        }

        static void Grab(Packet p)
        {
            #region Firewall Stuff
            int fwtier = 0;
            bool blocked = false;
            foreach (Upgrade u in Engine.upgradeList)
                if (u.name == "firewall3")
                {
                    fwtier = 3;
                    break;
                }
                else if (u.name == "firewall2")
                {
                    fwtier = 2;
                    break;
                }
                else if (u.name == "firewall1")
                {
                    fwtier = 1;
                    break;
                }

            Random r = new Random();
            int chance = r.Next(1, 11);

            switch (fwtier)
            {
                case 3:
                    if (chance <= 6)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }
                    break;
                case 2:
                    if (chance <= 4)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;

                    }
                    break;
                case 1:
                    if (chance <= 2)
                    {
                        Console.WriteLine("Firewall blocked an incoming connection!");
                        blocked = true;
                    }

                    break;
            }
            #endregion

            if (blocked || (Engine.frozen && !Engine.is_flooded))
            {
                Packet packet = new Packet(PacketType.GrabDenied);
                packet.data.Add(p.senderIP.ToString());
                packet.data.Add("blocked");
                master.Send(packet.ToBytes());
            }
            else if (p.data[1] == Engine.password)
            {
                Packet packet = new Packet(PacketType.GrabReturn);
                packet.data.Add(p.senderIP.ToString());
                List<Upgrade> upgrades = new List<Upgrade>();
                foreach (Upgrade u in Engine.upgradeList)
                    upgrades.Add(u);
                packet.packetObject = upgrades;
                packet.packetInt = Engine.bytes;
                Engine.bytes = 0;
                Engine.upgradeList.Clear();
                master.Send(packet.ToBytes());
            }
            else
            {
                Packet packet = new Packet(PacketType.GrabDenied);
                packet.data.Add(p.senderIP.ToString());
                packet.data.Add("false");
                master.Send(packet.ToBytes());
                Engine.ShowErrorNoSleep("A grab attempt was made with an invalid password!");
                Engine.PromptOut();
            }
        }

        static void GrabReturn(Packet p)
        {
            if (p.type == PacketType.GrabReturn)
            {
                Console.WriteLine("Grab successful! Bytes: " + p.packetInt);
                List<Upgrade> contents = (List<Upgrade>)p.packetObject;

                foreach (Upgrade u in contents)
                {
                    bool contains = false;
                    foreach (Upgrade up in Engine.upgradeList)
                        if (u.name == up.name)
                            contains = true;
                    if (!contains)
                    {
                        Engine.upgradeList.Add(u);
                        Console.WriteLine("Grabbed upgrade: " + u.name);
                    }


                }

                Engine.bytes += p.packetInt;

            }
            else if (p.data[1] == "blocked")
            {
                Engine.ShowErrorNoSleep("Grab attempt failed: Connection was actively refused by target system");
            }
            else if (p.data[1] == "false")
            {
                Engine.ShowErrorNoSleep("Grab attempt failed: Invalid password");
            }
            Engine.PromptOut();

        }

        static void ReturnRegInfo()
        {
            Packet packet = new Packet(PacketType.RegRequest);
            packet.senderIP = Packet.GetLocalIP4();
            packet.data.Add(name);
            master.Send(packet.ToBytes());
        }

        static void ScanResults(Packet p)
        {
            Console.WriteLine("\nNetwork IP scan results:");
            foreach (string s in p.data)
            {
                if (s == ipaddress.ToString())
                    Console.WriteLine(s + " (this device)");
                else
                    Console.WriteLine(s);
            }
            Console.WriteLine();
        }

        //static void InstallVirus(Packet p)
        //{
        //    Virus v = (Virus)p.packetObject;
        //    Engine.InstallVirus(v);
        //}
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
        static void ConnectionManagerMessage(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
        #endregion

    }
}
