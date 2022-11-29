using System.Runtime.Serialization;

namespace Cfg.Common
{
    [Serializable]
    public class DataGridException : Exception
    {
        public DataGridException()
        {
        }

        public DataGridException(string? message) : base(message)
        {
        }

        public DataGridException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DataGridException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}