using System.Runtime.Serialization;

namespace Api.Services
{
    [Serializable]
    internal class EmployeeIdNotFoundException : Exception
    {
        private int id;

        public EmployeeIdNotFoundException()
        {
        }

        public EmployeeIdNotFoundException(int id)
        {
            this.id = id;
        }

        public EmployeeIdNotFoundException(int id, string? message) : base(message)
        {
            this.id = id;
        }

        public EmployeeIdNotFoundException(int id, string? message, Exception? innerException) : base(message, innerException)
        {
            this.id = id;
        }

        protected EmployeeIdNotFoundException(int id, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.id = id;
        }
    }
}