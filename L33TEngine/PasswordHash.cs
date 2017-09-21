using System;
using System.IO;

namespace L33TEngine
{
    public class PasswordHash
    {
        public byte[] password;
        public string plaintext;
        public string name;

        public PasswordHash(string password, string name)
        {
            plaintext = password;
            this.password = new byte[password.Length];
            char[] split = password.ToCharArray();
            for (int i = 0; i < split.Length; i++)
            {
                this.password[i] = (byte)split[i];
            }

            this.name = name;
        }

        public PasswordHash(string fileName)
        {
            StreamReader sr = new StreamReader("files\\" + fileName);
            string input = sr.ReadLine();
            sr.Close();
            string[] split = input.Split(' ');
            password = new byte[split.Length];
            for(int i = 0; i < split.Length; i++)
            {
                password[i] = Convert.ToByte(split[i]);
            }
            Decrypt();
            plaintext = "";
            foreach (byte b in password)
            {
                plaintext += (char)b;
            }
        }

        private void Encrypt()
        {
            for (int i = 0; i < password.Length; i++)
            {
                password[i] -= 2;
            }
        }
        private void Decrypt()
        {
            for (int i = 0; i < password.Length; i++)
            {
                password[i] += 2;
            }
        }

        public void Save()
        {
            Encrypt();
            StreamWriter sw = new StreamWriter("files\\" + name + ".hsh");
            for (int i = 0; i < password.Length - 1; i++)
                sw.Write(password[i] + " ");
            sw.Write(password[password.Length - 1]);
            sw.Close();
        }

    }
}
