using System;
using System.Runtime.Serialization;

namespace IoCLite.Container.Exceptions
{
    public class NoItemRegisterdException : Exception
    {
        public NoItemRegisterdException()
        {
        }

        public NoItemRegisterdException(string message) : base(message)
        {
        }

        public NoItemRegisterdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoItemRegisterdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
