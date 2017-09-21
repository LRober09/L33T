using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L33TPackets
{
    public class Client
    {
        public Socket sck;
        public IPAddress ip;
        public bool registered;
        public string name;
        public Thread clientThread;
        public List<string> upgrades;

        public Client(Socket socket)
        {
            sck = socket;
            ip = Packet.GetLocalIP4();
            registered = false;
            upgrades = new List<string>();
        }

        public void SendPacket(Packet p)
        {
            sck.Send(p.ToBytes());
            
        }
    }
}
