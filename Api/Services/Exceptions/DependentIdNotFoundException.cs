using System.Runtime.Serialization;

namespace Api.Services
{
    [Serializable]
    internal class DependentIdNotFoundException : Exception
    {
        private int id;

        public DependentIdNotFoundException()
        {
        }

        public DependentIdNotFoundException(int id)
        {
            this.id = id;
        }

        public DependentIdNotFoundException(int id, string? message) : base(message)
        {
            this.id = id;
        }

        public DependentIdNotFoundException(int id, string? message, Exception? innerException) : base(message, innerException)
        {
            this.id = id;
        }

        protected DependentIdNotFoundException(int id, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.id = id;
        }
    }
}