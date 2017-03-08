using System;

namespace L33T
{
    public class UnexpectedPacketTypeException : Exception
    {
        public UnexpectedPacketTypeException()
            :base("The packet recieved was of an unexpected type!")
        { }
    }
}
