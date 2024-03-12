using System.Runtime.Serialization;

namespace Api.Services
{
    [Serializable]
    internal class SinglePartnerDependencyException : Exception
    {
        public SinglePartnerDependencyException()
        {
        }

        public SinglePartnerDependencyException(string? message) : base(message)
        {
        }

        public SinglePartnerDependencyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SinglePartnerDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}