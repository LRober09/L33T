using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace L33TClient
{
    public static class Tutorial
    {
        public static void Play()
        {
            FormatScreen();
            Console.WriteLine("Welcome to L33TH4X0R!");
            Thread.Sleep(5000);
            Console.WriteLine("L33TH4X0R is a console-based multiplayer game (over LAN).");
            Thread.Sleep(3000);
            Console.WriteLine("The purpose is to \"hack\" as many of the other players off the network as possible while upgrading your own abilities and defenses.");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();


            FormatScreen();
            Console.WriteLine("~~ Basic commands ~~");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(3000);
            Console.WriteLine("When you first join a server, you will encounter a command-line like the one below.");
            Console.WriteLine();
            Console.Write("C:\\Users\\ExampleUser>");
            Console.WriteLine();
            Thread.Sleep(7000);
            Console.WriteLine();
            Console.WriteLine("Gameplay is totally command driven, so type \"help\" or \"?\" and press enter to display the command list.");
            Thread.Sleep(3000);
            Console.WriteLine("If you need help with a specific command, type \"help <command>\"");
            Thread.Sleep(3000);
            Console.WriteLine("If you are constantly recieving a 'Invalid usage' error when using a command, use the above method to verify that you are executing it correctly.");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            FormatScreen();
            Console.WriteLine("~~ Upgrades/File Structure ~~");
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(5000);
            Console.WriteLine();
            Console.WriteLine("L33TH4X0R is structured like most other command-lines like Command Prompt in Windows and Terminal in Linux");
            Thread.Sleep(3000);
            Console.WriteLine("There are three different directories in the topmost directory: 'files', 'upgrades', and 'store'");
            Thread.Sleep(3000);
            Console.WriteLine("The 'dir' command will display all directories in the current directory");
            Thread.Sleep(3000);
            Console.WriteLine("The 'cd' command will allow you to change directories");
            Thread.Sleep(3000);
            Console.WriteLine("In order to interact with a file like an upgrade or virus, you must be in the directory of that file.");
            Thread.Sleep(3000);
            Console.WriteLine("Viruses are stored in the files directory and unpurchased upgrades are in the store directory");
            Thread.Sleep(3000);
            Console.WriteLine("To move back one directory, simply use the command 'cd ..'");
            Thread.Sleep(3000);
            Console.WriteLine("If you are exepriencing constant 'File not found' errors, verify that you are in the correct directory");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

        }





        private static void FormatScreen()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("-----| Tutorial |-----");
            Console.WriteLine();
        }
    }
}
