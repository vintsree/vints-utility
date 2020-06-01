namespace Vints.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XmlUtility
    {
        public static dynamic Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
            return parent;
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }

        /// <summary>
        /// Converts a .NET type into an XML compatible type - roughly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MapTypeToXmlType(Type type)
        {
            if (type == null)
                return null;

            if (type == typeof(string) || type == typeof(char))
                return "string";
            if (type == typeof(int) || type == typeof(int))
                return "integer";
            if (type == typeof(short) || type == typeof(byte))
                return "short";
            if (type == typeof(long) || type == typeof(long))
                return "long";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "datetime";

            if (type == typeof(float))
                return "float";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(float))
                return "single";

            if (type == typeof(byte))
                return "byte";

            if (type == typeof(byte[]))
                return "base64Binary";

            return null;
        }

    }
}
