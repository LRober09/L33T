using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace L33TPackets
{
    [Serializable]
    public class UserData
    {
        public string username;
        public string password;

        public UserData() { }
        public UserData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("user.data", FileMode.Create);
            bf.Serialize(fs, this);
            fs.Close();
        }
        public static UserData Load()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("user.data", FileMode.Open);
            UserData data = (UserData)bf.Deserialize(fs);
            fs.Close();
            return data;
        }
    }
}
