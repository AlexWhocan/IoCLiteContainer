using System;
using System.Runtime.Serialization;

namespace IoCLite.Container.Exceptions
{
    public class InvalidTypeRegistrationException : Exception
    {
        public InvalidTypeRegistrationException()
        {
        }

        public InvalidTypeRegistrationException(string message) : base(message)
        {
        }

        public InvalidTypeRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTypeRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
