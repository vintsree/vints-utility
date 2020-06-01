namespace Vints.Utility.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NullException<T> : Exception
    {
        public Type Type
        {
            get { return typeof(T); }
        }

        public NullException(string message) : base(message) { }
        public NullException(string message, Exception innerException) : base(message, innerException) { }
        protected NullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
