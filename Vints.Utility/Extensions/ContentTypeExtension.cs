namespace Vints.Utility.Extensions
{
    using Attributes;
    using Enums;
    using System.Reflection;

    public static class ContentTypeExtension
    {
        private static object GetMetadata(ContentType ct)
        {
            MemberInfo[] info = ct.GetType().GetMember(ct.ToString());
            if ((info != null) && (info.Length > 0))
            {
                object[] attrs = info[0].GetCustomAttributes(typeof(ContentTypeMetaData), false);
                if ((attrs != null) && (attrs.Length > 0))
                {
                    return attrs[0];
                }
            }
            return null;
        }

        public static string ToValue(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((ContentTypeMetaData)metadata).Value : ct.ToString();
        }

        public static bool IsText(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((ContentTypeMetaData)metadata).IsText : true;
        }

        public static bool IsBinary(this ContentType ct)
        {
            var metadata = GetMetadata(ct);
            return (metadata != null) ? ((ContentTypeMetaData)metadata).IsBinary : false;
        }
    }
}
