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
        public Dictionary<String, Object> Data { get; set; }


        public Packet(PacketType type)
        {
            Data = new Dictionary<string, Object>();
            this.Type = type;
        }

        public Packet(byte[] buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(buffer);

            Packet p = (Packet)formatter.Deserialize(stream);
            stream.Dispose();

            //set properties
            this.Type = p.Type;
            this.Data = p.Data;
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
        ConnectionResponse,
        ConnectionConfirm
    }
}


