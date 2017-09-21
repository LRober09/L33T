using L33TPackets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/*NOTES
 * 
 *  * attack random ip addresses
 * TEST FREEZING w/ VIRUS INSTALLATION
 * 
 * if boost is op, cause cpu speed detereoration
 * 
 * 
 * 
 */

namespace L33TEngine
{
    public class Engine
    {
        #region Declarations
        public static Socket master;
        public static string name;
        public static string password;
        public static int dirIndex = 0;
        public static int bytes = 0;
        public static double cpuSpeed = 2.2;
        public static List<Upgrade> upgradeList = new List<Upgrade>();
        public static int broadcastTimer = 0;
        public static int avTimer;
        public static List<Virus> installedViruses = new List<Virus>();
        public static List<string> muteList = new List<string>();
        public static string returner = "";
        public static bool frozen;
        public static bool is_flooded;
        #endregion

        #region Timers
        public static void timer()
        {

            for (int i = 0; i < 30; i++)
            {
                broadcastTimer++;
                Thread.Sleep(1000);
            }
            broadcastTimer = 0;
        }
        public static void avtimer1()
        {
            avTimer = 0;
            for (int i = 0; i < 60; i++)
            {
                if (!Engine.frozen)
                {
                    avTimer++;
                    
                    
                }
                else i--;

                Thread.Sleep(1000);
            }
            avTimer = 0;
        }
        public static void avtimer2()
        {
            avTimer = 0;
            for (int i = 0; i < 30; i++)
            {
                if (!Engine.frozen)
                {
                    avTimer++;
                    Thread.Sleep(1000);

                }
                else i--;

                Thread.Sleep(1000);
            }
            avTimer = 0;
        }
        public static void avtimer3()
        {
            avTimer = 0;
            for (int i = 0; i < 20; i++)
            {
                if (!Engine.frozen)
                {
                    avTimer++;
                    Thread.Sleep(1000);
                    
                }
                else i--;

                Thread.Sleep(1000);
            }
            avTimer = 0;
        }
        #endregion
        public static void DispGameInterface(string name)
        {
            Format();
            StartByteCount();
            string input;
            while (true)
            {
                PromptOut();
                input = Console.ReadLine();
                CommandManager(input);
            }
        }
        public static void PromptOut()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"C:\Users\" + name);

            #region Dir
            switch (dirIndex)
            {
                case 1:
                    Console.Write("\\upgrades");
                    break;
                case 2:
                    Console.Write("\\store");
                    break;
                case 3:
                    Console.Write("\\files");
                    break;
            }
            #endregion
            Console.Write(">");
        }
        private static void CommandManager(string cmd)
        {
            string[] command;


            command = cmd.Split(' ');
            switch (command[0].ToLower())
            {
                case "avscan":
                    Commands.AntivirusScan();
                    break;

                case "boost":
                    Commands.Boost();
                    break;

                case "broadcast":
                    if (command.Length >= 2)
                        Commands.Broadcast(command);
                    else ShowErrorNoSleep("Invalid usage of command!");
                    break;

                case "buy":
                    if (command.Length == 2)
                        Commands.BuyUpgrade(command);
                    else
                        ShowErrorNoSleep("Invalid usage of command!");
                    break;

                case "cd":
                    if (command.Length == 2)
                        dirIndex = Commands.ChangeDir(command, dirIndex);
                    else ShowErrorNoSleep("Invalid usage of command!");
                    break;

                case "clear":
                    Commands.Clear();
                    break;

                case "dir":
                    Commands.DispDir(dirIndex);
                    break;
                case "exit":
                    Commands.ExitDialog();
                    break;

                case "fcheck":
                    Commands.Fcheck(command);
                    break;

                case "fget":
                    Commands.Fget(command);
                    break;
                case "flood":
                    Commands.Flood(command);
                    break;
                case "freeze":
                    Commands.Freeze();
                    break;
                case "getbytes":
                    Console.WriteLine("Current byte count: " + bytes);
                    break;
                case "getcost":
                    Commands.GetCost(command);
                    break;
                case "getinfo":
                    Commands.DispInfo(command);
                    break;
                case "getspecs":
                    Commands.DispSpecs();
                    break;
                case "grab":
                    Commands.Grab(command);
                    break;
                case "help":
                case "?":
                    Commands.DispHelp(command);
                    break;

                case "ipconfig":
                    Console.WriteLine("Local IP4 Address: " + Packet.GetLocalIP4());
                    break;

                case "lcrack":
                    Commands.LCrack(command);
                    break;

                case "mute":
                    Commands.MutePlayer(command);
                    break;

                case "scan":
                    Commands.Scan();
                    break;
                case "suicide":
                    Virus v = new Virus(VirusType.Shutdown, VirusTier.Advanced, "death");
                    v.password = Engine.password;
                    Virus v2 = new Virus(VirusType.Inhibitor, VirusTier.Advanced, "inhibitor");
                    InstallVirus(v2);
                    InstallVirus(v);
                    break;

                case "test":
                    InstallVirus(new Virus(VirusType.Inhibitor, VirusTier.Advanced, "yoloswaggins"));
                    break;

                case "vgen":
                    Commands.VirusGen(command);
                    break;

                case "vsend":
                    Commands.VirusSend(command);
                    break;
                case "hax":
                    bytes += 10000;
                    break;
                case "meltingpoint":
                    cpuSpeed += 20;
                    break;
                case "": break;
                default:
                    ShowErrorNoSleep("'" + command[0] + "' is not recognized as an internal or external command, operable program, or batch file.");
                    break;
            }

        }

        #region Upgrades/Bytes
        private static void StartByteCount()
        {
            Thread byteThread = new Thread(IncBytes);
            byteThread.Start(1000);
        }
        private static void IncBytes(object delay)
        {
            for (; ; )
            {
                int de = (int)delay;
                if (!Engine.frozen)
                    bytes++;
                Thread.Sleep(de);
            }
        }
        public static void SendUpgrades()
        {
            Packet p = new Packet(PacketType.UpgradeUpdate);
            foreach (Upgrade u in upgradeList)
                p.data.Add(u.name);
            master.Send(p.ToBytes());
        }
        public static void ChangePassword()
        {
        A: Console.Write("Enter new password (max " + password.Length + 1 + " chars): ");

            string pass = Console.ReadLine();
            if (pass.Length <= password.Length + 1)
                password = pass;
            else
            {
                ShowErrorNoSleep("Password must be " + password.Length + 1 + " chars or less!");
                goto A;
            }
        }




        #endregion

        #region Viruses

        static void WormVirusThread(object id)
        {
            Virus virus = null;

            foreach (Virus v in installedViruses)
            {
                if ((int)id == v.id)
                    virus = v;
            }

            while (virus.installed == true)
            {
                if (!Engine.frozen || Engine.is_flooded)
                    cpuSpeed -= .1;
                int sleep = 60000;
                if (virus.tier == VirusTier.Advanced)
                    sleep = 30000;
                Thread.Sleep(sleep);

            }

            installedViruses.Remove(virus);
        }
        static void ShutdownVirus(object id)
        {
            Virus v = null;

            foreach (Virus vi in installedViruses)
                if (vi.id == (int)id)
                    v = vi;
            Console.WriteLine("shutdown pass: " + v.password);
            if (v.password == password)
            {
                Thread.Sleep(60000);


                if (v.installed)
                {

                    for (int i = 0; i < 1000; i++)
                    {
                        Console.ForegroundColor = (ConsoleColor)new Random().Next(0, 10);
                        Console.Write("GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER ");
                    }
                    Process.Start("C:\\Windows\\System32\\shutdown.exe", "/p");
                }
            }
            else
            {
                v.installed = false;
                installedViruses.Remove(v);
                ShowErrorNoSleep("A shutdown virus attempted to install with an incorrect password!");
                PromptOut();
            }
        }
        static void ChatVirus(object id)
        {

            Virus virus = null;

            foreach (Virus v in installedViruses)
            {
                if ((int)id == v.id)
                    virus = v;
            }
            int cycles = 100;
            if (virus.tier == VirusTier.Advanced)
                cycles = 200;

            for (int i = 0; i < cycles; i++)
            {
                if (virus.installed)
                {

                    Thread.Sleep(10000);
                    if (!Engine.frozen)
                        for (int i2 = 0; i2 < 10; i2++)
                            Console.WriteLine("SPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPAM");

                }
                else
                {
                    installedViruses.Remove(virus);
                    break;
                }
            }

        }
        static void ByteboostVirus(object id)
        {
            Virus virus = null;

            foreach (Virus v in installedViruses)
            {
                if ((int)id == v.id)
                    virus = v;
            }

            int cycles = 60;

            if (virus.tier == VirusTier.Advanced)
                cycles = 120;
            for (int i = 0; i < cycles; i++)
            {
                if (!Engine.frozen)
                    Engine.bytes += 10;
                Thread.Sleep(1000);
            }


            installedViruses.Remove(virus);




        }

        public static void InstallVirus(Virus v)
        {
            if (!Engine.frozen || Engine.is_flooded)
            {
                #region Antivirus check
                foreach (Upgrade u in upgradeList)
                {


                    if (u.name == "nortyantivirus")
                    {
                        int chance = new Random().Next(0, 10);

                        switch (v.tier)
                        {
                            case VirusTier.Basic:
                                if (chance != 9)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("A basic virus was removed during attempted installation!");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    goto A;
                                }
                                break;

                            case VirusTier.Advanced:
                                if (chance < 6)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("An advanced virus was removed during attempted installation!");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    goto A;
                                }

                                break;
                        }
                    }
                }
                #endregion
                #region Inhib check
                bool hasshutdown = false;
                if (v.type == VirusType.Shutdown)
                {
                    foreach (Virus vi in installedViruses)
                        if (vi.type == VirusType.Inhibitor)
                            hasshutdown = true;
                    if (!hasshutdown)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("A shutdown virus installation was attempted!");
                        Engine.PromptOut();
                        Console.ForegroundColor = ConsoleColor.Green;
                        goto A;
                    }
                }
                #endregion

                Virus virus = v;
                virus.installed = true;
                installedViruses.Add(virus);
                Thread virusThread;

                switch (virus.type)
                {
                    case VirusType.Worm:
                        virusThread = new Thread(WormVirusThread);
                        virusThread.Start(virus.id);
                        break;


                    case VirusType.Shutdown:
                        virusThread = new Thread(ShutdownVirus);
                        virusThread.Start(virus.id);
                        break;

                    case VirusType.Chat:
                        virusThread = new Thread(ChatVirus);
                        virusThread.Start(virus.id);
                        break;

                    case VirusType.Byteboost:
                        virusThread = new Thread(ByteboostVirus);
                        virusThread.Start(virus.id);
                        break;
                }



            A: int nuller;
            }
        }



        #endregion

        #region Misc
        public static void Format()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

        }
        public static void ShowError(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
            Thread.Sleep(1000);
        }
        public static void ShowErrorNoSleep(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        public static void CheckLocalFiles()
        {
            if (!Directory.Exists("upgrades"))
                Directory.CreateDirectory("upgrades");
            if (!Directory.Exists("store"))
                Directory.CreateDirectory("store");
            if (!Directory.Exists("files"))
                Directory.CreateDirectory("files");

            string[] upgradeDir = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\upgrades");
            foreach (string s in upgradeDir)
                File.Delete(s);

            string[] filesDir = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\files");
            foreach (string s in filesDir)
                File.Delete(s);

            Upgrade.PopulateUpgrades();

        } //don't forget to add upgrade files

        public static void LCrack(string fileName)
        {
            Format();
            Console.WriteLine("Loading password hash...");
            Thread.Sleep(Convert.ToInt32(20000 / cpuSpeed));
            #region Assignment
            PasswordHash ph;
            ph = new PasswordHash(fileName);
            #endregion

            Console.WriteLine("Loading default table...");

            #region Load Wordlist
            List<string> wordlist = new List<string>();
            string input = "";
            StreamReader sr = new StreamReader("wordlist.wrd");

            while (input != null)
            {
                wordlist.Add(input);
                input = sr.ReadLine();
            }
            sr.Close();

            #endregion

            Console.WriteLine("Cracking beginning in 3 seconds...");
            Thread.Sleep(3000);
            Random r = new Random();
            Format();
            double multiplier = 400000;
            multiplier *= ph.plaintext.Length - 5;
            multiplier /= cpuSpeed;
            for (int i = 0; i < Convert.ToInt32(multiplier); i++)
            {
                Console.Write("Trying: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(wordlist[r.Next(0, wordlist.Count)] + "\t\t[" + Guid.NewGuid().ToString() + "]");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.Write("Trying: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ph.plaintext + "\t\t[" + Guid.NewGuid().ToString() + "]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Password found: " + ph.plaintext);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Press enter to add this password to your password file...");
            Console.ReadLine();

            StreamWriter sw = new StreamWriter("passwords.txt", true);
            sw.WriteLine(ph.plaintext);
            sw.Close();

        A: int nuller;
        }

        public static void Flood()
        {
            is_flooded = true;

            Thread.Sleep(60000);
            is_flooded = false;
        }
        #endregion
    }

    public class Commands
    {
        public static void AntivirusScan()
        {
            if (Engine.upgradeList.Count != 0)
            {
                Random r = new Random();
                int chanceNum = r.Next(0, 10);

                foreach (Upgrade u in Engine.upgradeList)
                {
                    if (u.name == "nortyantivirus")
                    {
                        if (Engine.avTimer == 0)
                        {
                            for (int i = 0; i < Engine.installedViruses.Count; i++)
                            {
                                switch (Engine.installedViruses[i].tier)
                                {
                                    case VirusTier.Basic:
                                        if (chanceNum < 9)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("A basic virus was found and removed!");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Engine.installedViruses[i].installed = false;
                                            Engine.installedViruses.RemoveAt(i);
                                        }
                                        break;
                                    case VirusTier.Advanced:
                                        if (chanceNum < 6)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("An advanced virus was found and removed!");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Engine.installedViruses[i].installed = false;
                                            Engine.installedViruses.RemoveAt(i);
                                        }
                                        break;
                                }
                            }
                            Thread a = new Thread(Engine.avtimer3);
                            a.Start();
                        }
                        else Engine.ShowErrorNoSleep("Antivirus scanner is on cooldown: " + Engine.avTimer + " seconds elapsed...");


                    }
                    else if (u.name == "malwarebits")
                    {
                        if (Engine.avTimer == 0)
                        {
                            for (int i = 0; i < Engine.installedViruses.Count; i++)
                            {
                                switch (Engine.installedViruses[i].tier)
                                {
                                    case VirusTier.Basic:
                                        if (chanceNum < 6)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("A basic virus was found and removed!");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Engine.installedViruses[i].installed = false;
                                            Engine.installedViruses.RemoveAt(i);
                                        }
                                        break;
                                    case VirusTier.Advanced:
                                        if (chanceNum < 2)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("An advanced virus was found and removed!");
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Engine.installedViruses[i].installed = false;
                                            Engine.installedViruses.RemoveAt(i);
                                        }
                                        break;

                                }
                            }
                            Thread a = new Thread(Engine.avtimer2);
                            a.Start();
                        }
                        else Engine.ShowErrorNoSleep("Antivirus scanner is on cooldown: " + Engine.avTimer + " seconds elapsed...");
                    }
                    else if (u.name == "mcgafee")
                    {
                        if (Engine.avTimer == 0)
                        {
                            for (int i = 0; i < Engine.installedViruses.Count; i++)
                            {
                                if (chanceNum < 3 && Engine.installedViruses[i].tier == VirusTier.Basic)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("A basic virus was found and removed!");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Engine.installedViruses[i].installed = false;
                                    Engine.installedViruses.RemoveAt(i);
                                }
                            }
                            Thread a = new Thread(Engine.avtimer1);
                            a.Start();
                        }
                        else Engine.ShowErrorNoSleep("Antivirus scanner is on cooldown: " + Engine.avTimer + " seconds elapsed...");

                    }
                    else Engine.ShowErrorNoSleep("You do not have antivirus software installed!");
                }
            }
            else Engine.ShowErrorNoSleep("You do not have antivirus software installed!");
        }

        public static void Boost()
        {
            Console.WriteLine("Boosting in 3 seconds...");
            Thread.Sleep(3000);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 60; i >= 0; i--)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Boosting for " + i + " seconds...");
                Console.WriteLine("Byte: " + Engine.bytes);
                Engine.bytes += 1;
                Thread.Sleep(1000);
            }
            Engine.PromptOut();
        }

        public static void Broadcast(string[] message)
        {
            bool infected = false;
            foreach (Virus v in Engine.installedViruses)
            {
                if (v.type == VirusType.Chat)
                {
                    infected = true;
                }
            }

            if (!infected)
            {
                if (Engine.broadcastTimer == 0)
                {
                    Packet p = new Packet(PacketType.Broadcast);
                    p.data.Add(Engine.name);
                    string ms = "";
                    for (int i = 1; i < message.Length; i++)
                        ms += message[i] + " ";

                    p.data.Add(ms);
                    Engine.master.Send(p.ToBytes());

                    Thread t = new Thread(Engine.timer);
                    t.Start();
                }
                else Engine.ShowErrorNoSleep("You can only broadcast a message once every 30 seconds!");
            }
            else
            {
                for (int i = 0; i < 50000; i++)
                {
                    Console.ForegroundColor = (ConsoleColor)new Random().Next(0, 15);
                    Console.BackgroundColor = (ConsoleColor)new Random().Next(0, 10);
                    Console.Write("HUEHUEHUEHUEHUEHUEHUEHUEHUEHUEHUEHUEHUEHUE");
                }
                Console.ForegroundColor = ConsoleColor.Green;
            }

        }

        public static void BuyUpgrade(string[] cmd)
        {
            if (Engine.dirIndex == 2 && File.Exists("store\\" + cmd[1]))
            {
                int cost = Upgrade.GetUpgradePrice(cmd[1].Split('.')[0]);
                if (Engine.bytes >= cost)
                {
                    Engine.bytes -= cost;
                    Upgrade upgrade = new Upgrade("store\\" + cmd[1]);
                    if (upgrade.name == "cpuoverclock")
                    {
                        Engine.cpuSpeed += .2;
                    }
                    else if (upgrade.name == "pwordlength")
                    {
                        Engine.ChangePassword();
                    }
                    else
                    {
                        Engine.upgradeList.Add(upgrade);
                        File.Move("store\\" + cmd[1], "upgrades\\" + cmd[1]);
                        Engine.SendUpgrades();
                    }
                    Console.WriteLine("You purchased an upgrade: " + upgrade.name + ".up (" + cost + " bytes)");
                }
                else Engine.ShowErrorNoSleep("You do not have enough bytes to purchase this upgrade!");
            }
            else Engine.ShowErrorNoSleep("The system cannot find the file specified.");
        }

        public static int ChangeDir(string[] cmd, int index)
        {
            switch (index)
            {
                case 0:
                    if (cmd[1] == "upgrades")
                        index = 1;
                    else if (cmd[1] == "store")
                        index = 2;
                    else if (cmd[1] == "files")
                        index = 3;
                    else if (cmd[1] == "..") { }
                    else
                        Engine.ShowErrorNoSleep("The system cannot find the path specified.");
                    break;

                case 1:
                    if (cmd[1] == "..")
                        index = 0;
                    else Engine.ShowErrorNoSleep("The system cannot find the path specified.");
                    break;

                case 2:
                    if (cmd[1] == "..")
                        index = 0;
                    else Engine.ShowErrorNoSleep("The system cannot find the path specified.");

                    break;
                case 3:
                    if (cmd[1] == "..")
                        index = 0;
                    else Engine.ShowErrorNoSleep("The system cannot find the path specified.");
                    break;

            }

            return index;
        }

        public static void Clear()
        {
            Engine.Format();
        }

        public static void DispDir(int i)
        {
            string extension = "";
            switch (i)
            {
                case 3:
                    extension = "\\files";
                    break;

                case 2:
                    extension = "\\store";
                    break;

                case 1:
                    extension = "\\upgrades";
                    break;

                default:

                    break;
            }


            //switch (i)
            //{
            //    case 0:
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + extension);
            Console.WriteLine("\nDirectories Of C:\\Users\\" + Engine.name + extension + "\n");
            foreach (string s in dirs)
            {

                Console.WriteLine(s.Split('\\')[s.Split('\\').Length - 1]);
            }
            Console.WriteLine("\nFiles Of C:\\Users\\" + Engine.name + extension + "\n");
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + extension);
            foreach (string s in files)
            {
                Console.WriteLine(s.Split('\\')[s.Split('\\').Length - 1]);
            }
            //break;

            //    case 1:

            //        break;
            //}
        }

        public static void DispHelp(string[] cmd) //work to do
        {
            if (cmd.Length == 1)
            {
                Console.WriteLine("\nAll commands:\n");
                Console.WriteLine("avscan");
                Console.WriteLine("boost");
                Console.WriteLine("broadcast");
                Console.WriteLine("buy");
                Console.WriteLine("cd");
                Console.WriteLine("clear");
                Console.WriteLine("dir");
                Console.WriteLine("exit");
                Console.WriteLine("fcheck");
                Console.WriteLine("fget");
                Console.WriteLine("flood");
                Console.WriteLine("freeze");
                Console.WriteLine("getbytes");
                Console.WriteLine("getcost");
                Console.WriteLine("getinfo");
                Console.WriteLine("getspecs");
                Console.WriteLine("grab");
                Console.WriteLine("help");
                Console.WriteLine("ipconfig");
                Console.WriteLine("lcrack");
                Console.WriteLine("mute");
                Console.WriteLine("scan");
                Console.WriteLine("suicide");
                Console.WriteLine("vgen");
                Console.WriteLine("vsend");
                Console.WriteLine();
            }
            else if (cmd.Length == 2)
            {
                switch (cmd[1])
                {
                    case "avscan":
                        DispAdvHelp("avscan", "Use installed anti-virus software to scan the current system for viruses", "'avscan'");
                        break;
                    case "boost":
                        DispAdvHelp("boost", "Allocate all system resources to doubling byte production for 60 seconds", "'boost'");
                        break;
                    case "broadcast":
                        DispAdvHelp("broadcast", "Broadcast a message to all players (5 second cooldown)", "'broadcast <message>'");
                        break;
                    case "buy":
                        DispAdvHelp("buy", "Buy an upgrade with the .up extension", "'buy <upgradename>'");
                        break;
                    case "cd":
                        DispAdvHelp("cd", "Change directories", "cd <directory|..>'");
                        break;
                    case "clear":
                        DispAdvHelp("clear", "Clear the current screen", "'clear'");
                        break;
                    case "dir":
                        DispAdvHelp("dir", "List all directories at the current location", "'dir'");
                        break;
                    case "exit":
                        DispAdvHelp("exit", "Exit the application", "'exit'");
                        break;
                    case "fcheck":
                        DispAdvHelp("fcheck", "Check for unprotected files on a target system", "'fcheck <targetIP>'");
                        break;
                    case "fget":
                        DispAdvHelp("fget", "Retrieve any unprotected password hashes from a target system", "'fget <targetIP> <savename>'");
                        break;
                    case "flood":
                        DispAdvHelp("flood", "Flood a target system for 60 seconds, allowing connections to bypass a frozen system state", "'flood'");
                        break;
                    case "freeze":
                        DispAdvHelp("freeze", "Freezes the system for 60 seconds, preventing all connection attempts", "'freeze'");
                        break;
                    case "getbytes":
                        DispAdvHelp("getbytes", "Get the current byte count for the player", "'getbytes'");
                        break;
                    case "getcost":
                        DispAdvHelp("getcost", "Get the cost of a specific upgrade", "'getcost <upgradeFileName>'");
                        break;
                    case "getinfo":
                        DispAdvHelp("getinfo", "Get information about a specific upgrade", "'getinfo <upgradeFileName'");
                        break;
                    case "getspecs":
                        DispAdvHelp("getspecs", "Display current system specifications", "'getspecs'");
                        break;
                    case "grab":
                        DispAdvHelp("grab", "Transfer bytes and upgrades from the target machine", "'grab <targetIP> <targetpassword>'");
                        break;
                    case "help":
                        DispAdvHelp("help", "Display all commands or get help with a specific command", "'help' or 'help <command>'");
                        break;
                    case "ipconfig":
                        DispAdvHelp("ipconfig", "Display local IP4 address", "'ipconfig'");
                        break;
                    case "lcrack":
                        DispAdvHelp("lcracK", "Crack a specified password hash. Note: This will consume all machine memory during the cracking process.", "'lcrack <hashfile>'");
                        break;
                    case "mute":
                        DispAdvHelp("mute", "Mute a player (broadcast)", "'mute <playername>'");
                        break;
                    case "scan":
                        DispAdvHelp("scan", "Scan the network for all active IP addresses", "'scan'");
                        break;
                    case "suicide":
                        DispAdvHelp("suicide", "Committ suicide", "'suicide'");
                        break;
                    case "vgen":
                        DispAdvHelp("vgen", "Generate a virus file (consumes bytes)", "'vgen <worm|inhibitor|chat|byteboost> <savename>' or 'vgen shutdown <savename> <targetmachinepassword>'");
                        break;
                    case "vsend":
                        DispAdvHelp("vsend", "Send a virus to the specified IP address", "'vsend <filename> <ipaddress>'");
                        break;

                    default:
                        Engine.ShowErrorNoSleep("Command not recognized!");
                        break;
                }

            }
        }

        private static void DispAdvHelp(string cmd, string descrip, string usage)
        {
            Console.WriteLine("\nHelp: " + cmd + "\n");
            Console.WriteLine("Command description: " + descrip);
            Console.WriteLine("Command usage: " + usage);
            Console.WriteLine();
        }

        public static void DispInfo(string[] cmd)
        {
            if (cmd.Length != 2)
                Engine.ShowErrorNoSleep("Invalid usage of command!");
            else
            {
                if (Engine.dirIndex == 2 && File.Exists("store\\" + cmd[1]))
                {
                    string info = Upgrade.GetInfo(cmd[1]);
                    Console.WriteLine("Upgrade info: " + info);
                }
                else
                {
                    Engine.ShowErrorNoSleep("The system could not find the file specified.");
                }
            }

        }

        public static void DispSpecs()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nProcessor speed: " + Engine.cpuSpeed + "gHz\n");
            Console.WriteLine("Upgrades:");
            if (Engine.upgradeList.Count != 0)
                foreach (Upgrade u in Engine.upgradeList)
                    Console.WriteLine(u.name);
            else
                Engine.ShowErrorNoSleep("No upgrades have been purchased!");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void ExitDialog()
        {
            Console.Write("Are you sure? (y/n): ");
            string input = Console.ReadLine().ToLower();
            if (input == "yes" || input == "y")
            {
                Environment.Exit(0);
            }
        }

        public static void Fcheck(string[] cmd)
        {
            if (cmd.Length == 2)
            {
                try
                {
                    Packet p = new Packet(PacketType.FCheck);
                    IPAddress.Parse(cmd[1]);
                    p.data.Add(cmd[1]);
                    Engine.master.Send(p.ToBytes());
                    Console.WriteLine("Checking for vulnerable files...");
                    while (Engine.returner == "")
                    { /*do nothing*/}

                    if (Engine.returner == "true")
                        Console.WriteLine("Target system has vulnerable files!");
                    else if (Engine.returner == "false")
                        Console.WriteLine("Target system does not have vulnerable files!");
                    else Console.WriteLine("Target system actively refused connection!");

                    Engine.returner = "";
                }
                catch
                { Engine.ShowErrorNoSleep("An error occurred (invalid IP address?)"); }
            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void Fget(string[] cmd)
        {
            if (cmd.Length == 3)
            {
                try
                {
                    Packet p = new Packet(PacketType.FCheck);
                    IPAddress.Parse(cmd[1]);
                    p.data.Add(cmd[1]);
                    Engine.master.Send(p.ToBytes());
                    while (Engine.returner == "")
                    { /*do nothing*/}

                    if (Engine.returner == "true")
                    {
                        p = new Packet(PacketType.FGet);
                        p.data.Add(cmd[1]);
                        p.data.Add(cmd[2]);
                        Engine.master.Send(p.ToBytes());
                        Console.WriteLine("Sent file retrieval packet...");
                    }
                    else if (Engine.returner == "false")
                        Console.WriteLine("Target system does not have vulnerable files!");
                    else Console.WriteLine("Target system actively refused connection!");

                    Engine.returner = "";
                }
                catch
                { Engine.ShowErrorNoSleep("An error occurred (invalid IP address?)"); }
            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void Flood(string[] cmd)
        {

            if (cmd.Length == 2)
            {
                try
                {
                    IPAddress.Parse(cmd[1]);

                    #region CheckFrozen
                    Packet p = new Packet(PacketType.GetFrozen);
                    p.data.Add(cmd[1]);
                    Engine.master.Send(p.ToBytes());
                    while (Engine.returner == "")
                    { /* do nothing */}
                    #endregion

                    if (Engine.returner == "true")
                    {
                        p = new Packet(PacketType.Flood);
                        p.data.Add(cmd[1]);
                        Engine.master.Send(p.ToBytes());

                        for (int i = 60; i >= 0; i--)
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Flooding " + cmd[1] + " for " + i + " seconds...");
                            Thread.Sleep(1000);
                        }
                        Engine.PromptOut();
                    }
                    else
                    {
                        Console.WriteLine("The target system is not frozen!");
                    }




                }
                catch
                { Engine.ShowErrorNoSleep("An error occurred (invalid IP address?)"); }

            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void Freeze()
        {
            Console.WriteLine("Freezing in 3 seconds...");
            Thread.Sleep(3000);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Engine.frozen = true;
            for (int i = 60; i >= 0; i--)
            {
                Console.Clear();
                if (Engine.is_flooded)
                {
                    Console.Write("System is ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("flooded");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(" for " + i + " seconds...");
                }
                else
                    Console.WriteLine("System is frozen for " + i + " seconds...");
                Thread.Sleep(1000);
            }
            Engine.frozen = false;
            Engine.PromptOut();
        }

        public static void Grab(string[] cmd)
        {
            if (cmd.Length == 3)
            {
                try
                {
                    IPAddress.Parse(cmd[1]);
                    Console.WriteLine("Preparing grab packet...");
                    int sleep = 50000;
                    Thread.Sleep(Convert.ToInt32(sleep / Engine.cpuSpeed));
                    Packet p = new Packet(PacketType.Grab);
                    p.data.Add(cmd[1]);
                    p.data.Add(cmd[2]);
                    Engine.master.Send(p.ToBytes());
                    Console.WriteLine("Sent grab packet");
                }
                catch
                { Engine.ShowErrorNoSleep("An error occurred (invalid IP address?)"); }
            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void GetCost(string[] cmd)
        {
            if (cmd.Length != 2)
                Engine.ShowErrorNoSleep("Invalid usage of command!");
            else
            {
                if (Engine.dirIndex == 2 && File.Exists("store\\" + cmd[1]))
                {
                    int cost = Upgrade.GetUpgradePrice(cmd[1]);
                    Console.WriteLine("Cost of " + cmd[1] + ": " + cost + " bytes");
                }
                else
                {
                    Engine.ShowErrorNoSleep("The system could not find the file specified.");
                }
            }


        }

        public static void LCrack(string[] cmd)
        {
            if (cmd.Length == 2)
            {
                if (Engine.dirIndex == 3 && File.Exists("files\\" + cmd[1]))
                    Engine.LCrack(cmd[1]);
                else
                {
                    Engine.ShowErrorNoSleep("The system could not find the file specified.");
                }
            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void MutePlayer(string[] cmd)
        {
            if (cmd.Length == 1)
            {
                Engine.ShowErrorNoSleep("Invalid usage of command!");
            }
            else
            {
                string name = "";
                for (int i = 1; i < cmd.Length; i++)
                    name += cmd[i];
                Engine.muteList.Add(name);
                Console.WriteLine("Player muted: " + name);
            }
        }

        public static void Scan()
        {
            Packet p = new Packet(PacketType.ScanReq);
            Console.WriteLine("Scanning network for IP addresses...");
            Thread.Sleep(2000);
            Engine.master.Send(p.ToBytes());
            Thread.Sleep(200);
        }

        public static void VirusGen(string[] cmd)
        {
            if (cmd.Length == 3 && !cmd[2].Contains('.'))
            {

                int cost = 100;



                VirusTier tier = VirusTier.Basic;
                VirusType type;

                foreach (Upgrade u in Engine.upgradeList)
                {
                    if (u.name == "advvirusgen")
                        tier = VirusTier.Advanced;
                }

                switch (cmd[1].ToLower())
                {
                    case "worm":
                        type = VirusType.Worm;
                        break;
                    case "inhibitor":
                        type = VirusType.Inhibitor;
                        cost += 200;
                        break;
                    case "chat":
                        type = VirusType.Chat;
                        cost += 200;
                        break;
                    case "byteboost":
                        type = VirusType.Byteboost;
                        cost += 200;
                        break;

                    default:
                        Engine.ShowErrorNoSleep("Invalid virus type!");
                        goto A;
                }
                if (Engine.bytes < cost)
                {
                    Engine.ShowErrorNoSleep("You do not have enough bytes to generate this virus! (Costs " + cost + " bytes)");
                    goto A;
                }

                Console.WriteLine("Generating virus...");

                #region CPUbaseddelay
                int sleep = Convert.ToInt32(500000 / (Engine.cpuSpeed * 4));
                Thread.Sleep(sleep);
                #endregion



                Virus v = new Virus(type, tier, cmd[2]);
                v.Save();
                Console.WriteLine("Virus generated. Stored in C:\\Users\\" + Engine.name + "\\files");
                Engine.bytes -= cost;



            A: Thread.Sleep(1);




            }
            else if (cmd.Length == 4 && !cmd[2].Contains('.') && cmd[1] == "shutdown")
            {
                int cost = 400;



                VirusTier tier = VirusTier.Basic;
                VirusType type = VirusType.Shutdown;

                foreach (Upgrade u in Engine.upgradeList)
                {
                    if (u.name == "advvirusgen")
                        tier = VirusTier.Advanced;
                }
                if (Engine.bytes < cost)
                {
                    Engine.ShowErrorNoSleep("You do not have enough bytes to generate this virus! (Costs " + cost + " bytes)");
                    goto A;
                }

                Console.WriteLine("Generating virus...");
                #region CPUbaseddelay
                int sleep = Convert.ToInt32(500000 / (Engine.cpuSpeed * 4));
                Thread.Sleep(sleep);
                #endregion



                Virus v = new Virus(type, tier, cmd[2]);
                v.password = cmd[3];
                v.Save();
                Console.WriteLine("Virus generated. Stored in C:\\Users\\" + Engine.name + "\\files");
                Engine.bytes -= cost;





            A: Thread.Sleep(1);
            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }

        public static void VirusSend(string[] cmd)
        {
            if (cmd.Length == 3)
            {
                if (Engine.dirIndex == 3 && File.Exists("files\\" + cmd[1]))
                {
                    Virus v = new Virus("files\\" + cmd[1]);
                    Packet p = new Packet(PacketType.Virus);
                    p.packetObject = v;
                    p.data.Add(cmd[2]);
                    Engine.master.Send(p.ToBytes());
                    File.Delete("files\\" + cmd[1]);
                    Console.WriteLine("Sent virus to: " + cmd[2]);
                }
                else Engine.ShowErrorNoSleep("The system could not find the file specified.");


            }
            else Engine.ShowErrorNoSleep("Invalid usage of command!");
        }



    }
}
