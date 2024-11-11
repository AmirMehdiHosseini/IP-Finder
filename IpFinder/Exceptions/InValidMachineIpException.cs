using System.Runtime.Serialization;

namespace IpFinder.Exceptions
{
    [Serializable]
    internal class InValidMachineIpException : Exception
    {
        public InValidMachineIpException()
        {
        }

        public InValidMachineIpException(string? message) : base(message)
        {
        }

        public InValidMachineIpException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InValidMachineIpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}