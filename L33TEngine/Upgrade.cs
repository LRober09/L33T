using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace L33TEngine
{

    [Serializable]
    public class Upgrade
    {
        public UpgradeType type;
        public UpgradeTier tier;
        public string name;
        public int cost;
        public string info;
        public static List<Upgrade> upgradeList;// = new List<string> { "antivirus1", "antivirus2", "antivirus3", "cpuoverclock", "firewall1", "firewall2", "firewall3", "scanner1", "scanner2" };


        public Upgrade(UpgradeType type)
        {
            this.type = type;
            this.tier = UpgradeTier.Tier1;
        }
        public Upgrade(UpgradeType type, UpgradeTier tier)
        {
            this.type = type;
            this.tier = tier;
        }
        public Upgrade(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Upgrade u = (Upgrade)bf.Deserialize(fs);
            fs.Close();
            this.type = u.type;
            this.tier = u.tier;
            this.name = u.name;
            this.cost = u.cost;
            this.info = u.info;

        }
        public Upgrade(string name, int cost, string info)
        {
            this.name = name;
            this.cost = cost;
            this.info = info;
            type = UpgradeType.None;
            tier = UpgradeTier.None;
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("store\\" + name + ".up", FileMode.Create);
            bf.Serialize(fs, this);
            fs.Close();
        }



        public static void PopulateUpgrades()
        {
            //if (!File.Exists("upgradelist.data"))
            //    File.Create("upgradelist.data");

           
            StreamReader sr = new StreamReader("upgradelist.data");
            upgradeList = new List<Upgrade>();
            List<string> upgrades = new List<string>();
            string line = "";

            do
            {
                line = sr.ReadLine();
                if (line != null)
                    upgrades.Add(line);
            }
            while (line != null);

            sr.Close();
            
            foreach (string s in upgrades)
            {
                string[] data = s.Split(',');
                Upgrade u = new Upgrade(data[0], Convert.ToInt16(data[1]), data[2]);
                u.Save();
                upgradeList.Add(u);

            }



        }
        public static int GetUpgradePrice(string upgrade)
        {
            foreach (Upgrade u in upgradeList)
            {
                if (u.name == upgrade.Split('.')[0])
                    return u.cost;
            }
            return -1;
        }
        public static string GetInfo(string upgrade)
        {
            foreach (Upgrade u in upgradeList)
            {
                if (u.name == upgrade.Split('.')[0])
                    return u.info;
            }
            return "nullinfo";
        }

    }

    public enum UpgradeType
    {
        Scanner,
        None
    }

    public enum UpgradeTier
    {
        Tier1,
        Tier2,
        Tier3,
        None
    }
}
