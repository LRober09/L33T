using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L33T
{
    public static class Control
    {
        private static List<User> users = new List<User>();

        public static void Join(Packet packet)
        {

        }

        /// <summary>
        /// Checks to see if there's a user with the given username already in the user list. If there isn't, adds that user to the list
        /// </summary>
        /// <param name="packet">Packet from client containing username</param>
        /// <returns>False if the user was not in the list</returns>
        public static bool ContainsUser(Packet packet)
        {
            string username = packet.Get("username").ToString();
            bool contains = users.Count(user => user.Name == username) > 0;
            if (!contains)
            {
                users.Add(new User(packet.SenderID, username));
            }
            return contains;
        }

        /// <summary>
        /// Checks to see if there is a user with the provided ID, and removes them from the list if there is
        /// </summary>
        /// <param name="id">The client/user id for the user to remove</param>
        /// <returns>True if a user was removed, false if the list did not contain that user</returns>
        public static bool RemoveUser(string id)
        {
            bool contains = users.Count(user => user.ID == id) > 0;
            if (contains)
            {
                users.Remove(users.Find(user => user.ID == id));
                return true;
            }
            else return false;
        }
    }
}
