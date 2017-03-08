using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using L33T;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(Annoying);
            thread.Start();

            while(true)
            {
                if(Console.ReadLine().ToLower() == "stop")
                {
                    thread.Abort();
                }
            }
            
        }

        private static void Annoying()
        {
            while (true)
            {
                Console.WriteLine("Annoyed yet?");
                Thread.Sleep(1000);
            }
        }
    }
}
