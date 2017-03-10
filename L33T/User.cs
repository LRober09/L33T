using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L33T
{
    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public User(string id)
        {
            this.ID = id;
        }

        public User(string id, string username)
        {
            this.ID = id;
            this.Name = username;
        }
    }
}
