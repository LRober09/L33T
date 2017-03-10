using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace L33T
{

    [Serializable]
    public class Packet
    {
        public PacketType Type { get; set; }
        public string SenderID { get; set; }

        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Packet(PacketType type)
        {
            data = new Dictionary<string, Object>();
            this.Type = type;
            this.SenderID = "server";
        }

        public Packet(PacketType type, string senderId)
        {
            data = new Dictionary<string, Object>();
            this.Type = type;
            this.SenderID = senderId;
        }

        public Packet(byte[] buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(buffer);

            Packet p = (Packet)formatter.Deserialize(stream);
            stream.Dispose();

            //set properties
            this.Type = p.Type;
            this.data = p.data;
            this.SenderID = p.SenderID;
        }

        public void Add(string key, object data)
        {
            this.data.Add(key.ToLower(), data);
        }

        public object Get(string key)
        {
            return this.data[key.ToLower()];
        }

        public byte[] Serialize()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            byte[] data = stream.GetBuffer();
            stream.Dispose();

            return data;
        }
    }

    [Serializable]
    public enum PacketType
    {
        Acknowledge,
        Register,
        Join,
        CheckUsername
    }
}


