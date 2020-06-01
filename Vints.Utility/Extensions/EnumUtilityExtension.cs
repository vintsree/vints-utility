namespace Vints.Utility.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The EnumRegistry class is intended to encapsulate high performance, scalable best practices for common uses of EnumUtility.
    /// </summary>
    public static class EnumUtilityExtension
    {
        public const string DESCRIPTION = "Description";
        public const string ATTRIBUTE_VALUE = "AttributeValue";

        public static Dictionary<PropertyInfo, Dictionary<Enum, string>> Descriptions { get; set; }
        public static Dictionary<PropertyInfo, Dictionary<Enum, Type>> Types { get; set; }

        /// <summary>
        /// Register is an Extension method for EnumUtility, in order to register the attribute info.
        /// </summary>
        /// <param name="utility"></param>
        /// <param name="attributeTexts">string value attributes</param>
        /// <param name="attributeTypes">Type value attributes</param>
        public static void Register(this EnumUtility utility, ref Dictionary<PropertyInfo, Dictionary<Enum, string>> descriptions, ref Dictionary<PropertyInfo, Dictionary<Enum, Type>> types)
        {
            if (utility == null) return;

            descriptions = Descriptions;
            types = Types;
        }
    }
}
