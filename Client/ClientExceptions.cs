using System;

namespace Client
{
    public class ClientNotInitializedException : Exception
    {
        public ClientNotInitializedException()
            : base("A connection was attempted before the client was initialized!")
        {

        }
    }
    
}
