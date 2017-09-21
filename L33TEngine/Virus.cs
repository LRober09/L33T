using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace L33TEngine
{
    [Serializable]
    public class Virus
    {
        public VirusType type;
        public VirusTier tier;
        public string name;
        public bool installed;
        public int id;
        public string password;

        public Virus(VirusType type, VirusTier tier, string name)
        {
            this.type = type;
            this.tier = tier;
            this.name = name;
            installed = false;

            id = new Random().Next(100000, 900000);
        }
        public Virus(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Virus v = (Virus)bf.Deserialize(fs);
            fs.Close();
            this.type = v.type;
            this.name = v.name;
            this.tier = v.tier;
            installed = false;
            this.id = v.id;
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("files\\" + name + ".vs", FileMode.Create);
            bf.Serialize(fs, this);
            fs.Close();
        }

    }

    public enum VirusType
    {
        Worm,
        Inhibitor,
        Shutdown,
        Chat,
        Byteboost,
        //Doge
    }
    public enum VirusTier
    {
        Basic,
        Advanced
    }
}
