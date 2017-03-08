using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L33T
{
    public static class Game
    {
        public static Dictionary<string, string> PlayerIds = new Dictionary<string, string>();

        public static void Join(Packet packet)
        {
            PlayerIds.Add(packet.SenderID, packet.Data["name"].ToString());
        }

        public static bool CheckUsername(string username)
        {
            return PlayerIds.ContainsValue(username);
        }
    }
}
