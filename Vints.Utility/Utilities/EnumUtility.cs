namespace Vints.Utility
{
    using Attributes;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    /// <summary>
    /// The EnumUtility class is intended to encapsulate high performance, scalable best practices for common uses of Enum.
    /// </summary>
    public sealed class EnumUtility
    {
        private static readonly Lazy<EnumUtility> instance = new Lazy<EnumUtility>(() => new EnumUtility(), LazyThreadSafetyMode.PublicationOnly);
        private const string DESCRIPTION = "Description";
        private static readonly string[] attributeSeparators;

        private static readonly PropertyInfo description;
        private static readonly Dictionary<PropertyInfo, Dictionary<Enum, string>> attributeDescriptions;
        private static readonly Dictionary<PropertyInfo, Dictionary<Enum, Type>> attributeTypes;

        public static EnumUtility Instance { get { return instance.Value; } }

        private EnumUtility()
        {

        }
        static EnumUtility()
        {
            attributeSeparators = new string[] { ", " };
            description = typeof(DescriptionAttribute).GetProperty(DESCRIPTION);
            Instance.Register(ref attributeDescriptions, ref attributeTypes);
            if (attributeDescriptions == null)
                attributeDescriptions = new Dictionary<PropertyInfo, Dictionary<Enum, string>>() {
                {
                  typeof(DescriptionAttribute).GetProperty(EnumUtilityExtension.DESCRIPTION), new Dictionary<Enum, string>() }
                };
            return;
        }

        #region Methods
        /// <summary>
        /// Retrieves the type of the member values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> The type of the values of the enumeration.</returns>
        public Type GetUnderlyingType<T>()
        {
            return (Enum.GetUnderlyingType(typeof(T)));
        }

        /// <summary>
        ///  Retrieves the names of the members.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> An array containing the member names.</returns>
        public string[] GetNames<T>()
        {
            return (Enum.GetNames(typeof(T)));
        }

        /// <summary>
        /// Retrieves the name of the value.
        /// </summary>
        /// <param name="value">The value whose name is desired.</param>
        /// <returns>The name of the value.</returns>
        public string GetName(Enum value)
        {
            return (Enum.GetName(value.GetType(), value));
        }

        /// <summary>
        ///  Formats the value.
        /// </summary>
        /// <param name="value">The value whose formatted value is desired.</param>
        /// <param name="format"> The format to use.</param>
        /// <returns>A string containing the value after formatting.</returns>
        public string Format(Enum value, string format)
        {
            return (Enum.Format(value.GetType(), value, format));
        }

        /// <summary>
        ///  Retrieves the member from the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"> value must be a numeric as a string</param>
        /// <returns> Enum Member containing the member value</returns>
        public T GetMember<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw (new ArgumentNullException("value must not be null/empty"));
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$"))
                throw (new ArgumentNullException(value + " value must contains all numeric"));
            if (Enum.IsDefined(typeof(T), value))
                throw (new ArgumentNullException(value + " value not exists in the enum"));
            return (T)Enum.ToObject(typeof(T), Convert.ToInt32(value));
        }

        /// <summary>
        ///  Retrieves the member from the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns> Enum Member containing the member value</returns>
        public T GetMember<T>(int value)
        {
            if (Enum.IsDefined(typeof(T), value))
                throw (new ArgumentNullException(value + " value not exists in the enum"));
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        ///  Retrieves the values of the members.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An array containing the member values.</returns>
        public T[] GetValues<T>()
        {
            return ((T[])Enum.GetValues(typeof(T)));
        }

        /// <summary>
        /// Determines whether or not the value exists in the enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns> Whether or not the value was found.</returns>
        public bool IsDefined<T>(object value)
        {
            return (Enum.IsDefined(typeof(T), value));
        }

        /// <summary>
        /// Determines whether or not the name exists in the enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The string to check.</param>
        /// <param name="ignoreCase">Whether or not to perform a case-insensitive search.</param>
        /// <returns>Whether or not the name was found.</returns>
        public bool IsDefined<T>(string name, bool ignoreCase)
        {
            bool result = false;
            if (!ignoreCase)
            {
                result = IsDefined<T>(name);
            }
            else
            {
                string[] names = Enum.GetNames(typeof(T));
                for (int runner = 0; !result && (runner < names.Length); runner++)
                {
                    result = StringComparer.CurrentCultureIgnoreCase.Compare(name, names[runner]) == 0;
                }
            }

            return (result);
        }

        /// <summary>
        /// Retrieves the value associated with the provided name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The string to parse.</param>
        /// <returns>The requested value if it exists.</returns>
        public T Parse<T>(string name)
        {
            return ((T)Enum.Parse(typeof(T), name));
        }

        /// <summary>
        /// Retrieves the value associated with the provided name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The string to parse.</param>
        /// <param name="ignoreCase">Whether or not to perform a case-insensitive parse.</param>
        /// <returns>The requested value if it exists.</returns>
        public T Parse<T>(string name, bool ignoreCase)
        {
            return ((T)Enum.Parse(typeof(T), name, ignoreCase));
        }

        /// <summary>
        ///  Retrieves the value associated with the provided name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"> The string to parse.</param>
        /// <param name="result">Parameter to hold the requested value (or the default if the name wasn't found).</param>
        /// <returns>Whether or not the name was found.</returns>
        public bool TryParse<T>(string name, out T result)
        {
            bool returnValue = false;
            try
            {
                result = (T)Enum.Parse(typeof(T), name);
                returnValue = true;
            }
            catch (ArgumentNullException)
            {
                result = Default<T>();
            }
            catch (ArgumentException)
            {
                result = Default<T>();
            }
            return (returnValue);
        }

        /// <summary>
        /// Retrieves the value associated with the provided name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The string to parse.</param>
        /// <param name="ignoreCase">Whether or not to perform a case-insensitive parse.</param>
        /// <param name="result">Parameter to hold the requested value (or the default if the name wasn't found).</param>
        /// <returns>Whether or not the name was found.</returns>
        public bool TryParse<T>(string name, bool ignoreCase, out T result)
        {
            bool returnValue = false;
            try
            {
                result = (T)Enum.Parse(typeof(T), name, ignoreCase);
                returnValue = true;
            }
            catch (ArgumentNullException)
            {
                result = Default<T>();
            }
            catch (ArgumentException)
            {
                result = Default<T>();
            }

            return (returnValue);
        }

        /// <summary>
        /// Get Descriptions of DescriptionAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<string> GetDescriptions(Type type)
        {
            var descriptions = new List<string>();
            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                foreach (DescriptionAttribute fd in fds)
                {
                    descriptions.Add(fd.Description);
                }
            }
            return descriptions;
        }

        /// <summary>
        ///  Retrieve the value of an attribute's property on an enum value, if any.
        /// </summary>
        /// <param name="value">The value whose attribute text is desired.</param>
        ///<returns>The value of the Property or an empty string.</returns>
        ///<exception cref="System.ArgumentException">The Property is not from an Attribute</exception>
        public string GetDescription(Enum value)
        {
            PropertyInfo Property = description;
            if (Property == null)
            {
                throw (new ArgumentNullException("Property", "The Property must not be null"));
            }

            if (!Property.ReflectedType.IsSubclassOf(typeof(Attribute)))
            {
                throw (new ArgumentException("The Property must be from an Attribute"));
            }

            if (!attributeDescriptions[Property].ContainsKey(value))
            {
                attributeDescriptions[Property][value] = GetPropertyValue(Property, value);
            }
            return (attributeDescriptions[Property][value]);
        }


        /// <summary>
        ///  Retrieve the value of an attribute's property on an enum value, if any.
        /// </summary>
        /// <param name="value">The value whose attribute texts is desired.</param>
        /// <param name="Property">The property of an attribute to use as the source for the attribute Texts.</param>
        ///<returns>The value of the Property or an empty string.</returns>
        ///<exception cref="System.ArgumentException">The Property is not from an Attribute</exception>
        public string GetAttributeValue(Enum value, PropertyInfo Property)
        {
            if (Property == null)
            {
                throw (new ArgumentNullException("Property", "The Property must not be null"));
            }

            if (!Property.ReflectedType.IsSubclassOf(typeof(Attribute)))
            {
                throw (new ArgumentException("The Property must be from an Attribute"));
            }

            if (!attributeDescriptions.ContainsKey(Property))
            {
                lock (attributeDescriptions)
                {
                    attributeDescriptions[Property] = new Dictionary<Enum, string>();
                }
            }
            if (!attributeDescriptions[Property].ContainsKey(value))
            {
                lock (attributeDescriptions)
                {
                    attributeDescriptions[Property][value] = GetPropertyValue(Property, value);
                }
            }

            return (attributeDescriptions[Property][value]);
        }

        /// <summary>
        ///  Retrieve the value of an attribute's property on an enum value, if any.
        /// </summary>
        /// <param name="value">The value whose alternate text is desired.</param>
        /// <param name="type">The property of an attribute to use as the source for the attribute texts.</param>
        ///<returns>The value of the Property or an empty string.</returns>
        ///<exception cref="System.ArgumentException">The Property is not from an Attribute</exception>
        public string GetDescription(Enum value, Type type)
        {
            PropertyInfo Property = type.GetProperty(DESCRIPTION);
            if (Property == null)
            {
                throw (new ArgumentNullException("Property", "The Property must not be null"));
            }

            if (!Property.ReflectedType.IsSubclassOf(typeof(Attribute)))
            {
                throw (new ArgumentException("The Property must be from an Attribute"));
            }

            if (!attributeDescriptions.ContainsKey(Property))
            {
                lock (attributeDescriptions)
                {
                    attributeDescriptions[Property] = new Dictionary<Enum, string>();
                }
            }

            if (!attributeDescriptions[Property].ContainsKey(value))
            {
                lock (attributeDescriptions)
                {
                    attributeDescriptions[Property][value] = GetPropertyValue(Property, value);
                }
            }

            return (attributeDescriptions[Property][value]);
        }

        /// <summary>
        ///  Retrieve the value of an attribute's property on an enum value, if any whose type belongs Type.
        /// </summary>
        /// <param name="value">The value whose alternate text is desired.</param>
        /// <param name="type">The property of an attribute to use as the source for the attribute texts.</param>
        ///<returns>The value of the Property or an null.</returns>
        ///<exception cref="System.ArgumentException">The Property is not from an Attribute</exception>
        public Type GetAttributeValue(Enum value, Type type)
        {
            PropertyInfo Property = type.GetProperty("AttributeValue");
            if (Property == null)
            {
                throw (new ArgumentNullException("Property", "The Property must not be null"));
            }

            if (!Property.ReflectedType.IsSubclassOf(typeof(Attribute)))
            {
                throw (new ArgumentException("The Property must be from an Attribute"));
            }

            if (!attributeTypes.ContainsKey(Property))
            {
                lock (attributeTypes)
                {
                    attributeTypes[Property] = new Dictionary<Enum, Type>();
                }
            }

            if (!attributeTypes[Property].ContainsKey(value))
            {
                lock (attributeTypes)
                {
                    attributeTypes[Property][value] = GetTypePropertyValue(Property, value) as Type;
                }
            }
            return (attributeTypes[Property][value]);
        }

        /// <summary>
        /// Retrieves the default value of the enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The default value of the enumeration.</returns>
        /// <remarks>The standard default for an enum is 0, but this method will honor a Attributes.EnumDefaultValueAttribute if one has been applied by the developer of the enum.</remarks>
        public T Default<T>()
        {
            T result;
            EnumDefaultValueAttribute.GetDefaultValue<T>(out result);
            return (result);
        }

        #endregion Methods

        #region Private Methods
        /// <summary>
        /// Get Property Value 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetPropertyValue(PropertyInfo property, Enum value)
        {
            StringBuilder sbDescription = new StringBuilder();
            foreach (string val in value.ToString().Split(attributeSeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (Attribute att in (Attribute[])value.GetType().GetField(val).GetCustomAttributes(property.DeclaringType, false))
                {
                    if (sbDescription.Length > 0)
                    {
                        sbDescription.Append(attributeSeparators[0]);
                    }
                    sbDescription.Append(property.GetValue(att, null).ToString());
                }
            }
            return (sbDescription.ToString());
        }

        /// <summary>
        /// Get Type Property Value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object GetTypePropertyValue(PropertyInfo property, Enum value)
        {
            Attribute[] attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(property.DeclaringType, false) as Attribute[];
            return attributes.Count() > 0 ? property.GetValue(attributes.FirstOrDefault(), null) : null;
        }
        #endregion Private Methods
    }
}
