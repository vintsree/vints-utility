namespace Vints.Utility.Exceptions
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    ///   Represents errors that occur during <see cref="HttpResponseMessage"/>
    /// </summary>
    [Serializable]
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.StatusCode = EnumUtility.Instance.Parse<HttpStatusCode>(info.GetString("HttpResponseException.StatusCode"));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("HttpResponseException.StatusCode", this.StatusCode.ToString());
        }
    }
}
