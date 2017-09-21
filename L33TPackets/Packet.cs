using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace L33TPackets
{
    [Serializable]
    public class Packet
    {
        public IPAddress senderIP;
        public List<string> data;
        public PacketType type;
        public object packetObject;
        public bool packetBool;
        public int packetInt;

        public Packet(byte[] Buffer)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(Buffer);
            Packet p = (Packet)bf.Deserialize(ms);
            ms.Close();
            this.senderIP = p.senderIP;
            this.data = p.data;
            this.type = p.type;
            this.packetObject = p.packetObject;
            this.packetBool = p.packetBool;
            this.packetInt = p.packetInt;
            //variable assignment here

        }
        public Packet(PacketType packetType)
        {
            this.type = packetType;
            data = new List<string>();
            senderIP = Packet.GetLocalIP4();
            packetObject = null;
            
        }
        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            ms.Close();
            return ms.ToArray();
        }


        #region Static
        public static IPAddress GetLocalIP4()
        {
            IPAddress lip = null;
            IPHostEntry host;

            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    lip = ip;
                    break;
                }
            }


            return lip;
        }
        public static void SendRegRequest(Client client)
        {
            Packet p = new Packet(PacketType.RegRequest);
            p.senderIP = GetLocalIP4();
            client.SendPacket(p);
        }
        
        #endregion
    }

    public enum PacketType
    { 
        Broadcast,
        RegRequest,
        ScanReq,
        UpgradeUpdate,
        Virus,
        FCheck,
        FCheckReturn,
        FGet,
        FGetReturn,
        FGetDenied,
        Flood,
        GetFrozen,
        GetFrozenReturn,
        Grab,
        GrabReturn,
        GrabDenied
    }
}
