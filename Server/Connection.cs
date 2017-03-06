using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Connection
    {
        public String ID { get; set; }
        public Socket Socket { get; set; }
        public Thread ClientThread { get; set; }
        public IPAddress IP { get; set; }
        public bool Admin { get; set; }

        public Connection(Socket acceptedSocket)
        {
            ID = Guid.NewGuid().ToString();
            Socket = acceptedSocket;
            Admin = false;
        }
    }
}
