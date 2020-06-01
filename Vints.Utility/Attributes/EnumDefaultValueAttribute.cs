namespace Vints.Utility.Attributes
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The EnumDefaultValue is used to set a default member (value) at compile time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class EnumDefaultValueAttribute : Attribute
    {
        private static readonly Dictionary<Type, object> defaults = new Dictionary<Type, object>();
        private readonly object value;

        ///<summary>
        ///   Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        ///<param name="Value">
        ///   The value to use as the default for the enum.
        ///</param>
        public EnumDefaultValueAttribute(Type Value)
            : base()
        {
            this.value = Value;
            return;
        }

        ///<summary>
        ///   Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        ///<param name="Value">
        ///   The value to use as the default for the enum.
        ///</param>
        public EnumDefaultValueAttribute(Enum Value)
            : base()
        {
            this.value = Value;
            return;
        }

        ///<summary>
        ///  Specifies that the provided value should be the default for the affected enum.
        ///</summary>
        ///<param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(SByte Value)
            : base()
        {
            this.value = Value;
            return;
        }

        ///<summary>
        ///  Specifies that the provided value should be the default for the affected enum.
        ///</summary>
        ///<param name="Value">
        ///  The value to use as the default for the enum.
        ///</param>
        public EnumDefaultValueAttribute(Byte Value)
            : base()
        {
            this.value = Value;
            return;
        }

        ///<summary>
        ///   Specifies that the provided value should be the default for the affected enum.
        ///</summary>
        ///<param name="Value">
        ///    The value to use as the default for the enum.
        ///</param>

        public EnumDefaultValueAttribute(Int16 Value)
            : base()
        {
            this.value = Value;
            return;
        }

        ///<summary>
        /// Specifies that the provided value should be the default for the affected enum.
        ///</summary>
        ///<param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(UInt16 Value)
            : base()
        {
            this.value = Value;
            return;
        }

        /// <summary>
        /// Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        /// <param name="Value"> The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(Int32 Value)
            : base()
        {
            this.value = Value;
            return;
        }

        /// <summary>
        /// Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        /// <param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(UInt32 Value)
            : base()
        {
            this.value = Value;
            return;
        }

        /// <summary>
        /// Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        /// <param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(Int64 Value)
            : base()
        {
            this.value = Value;
            return;
        }

        /// <summary>
        /// Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        /// <param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(UInt64 Value)
            : base()
        {
            this.value = Value;

            return;
        }


        /// <summary>
        /// Specifies that the provided value should be the default for the affected enum.
        /// </summary>
        /// <param name="Value">The value to use as the default for the enum.</param>
        public EnumDefaultValueAttribute(String Value)
            : base()
        {
            this.value = Value;
            return;
        }

        /// <summary>
        /// Performs ToString() on the value.
        /// </summary>
        /// <returns>The value as a string.</returns>
        public override string ToString()
        {
            return (this.value.ToString());
        }


        /// <summary>
        /// Retrieve the default value for the enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Result"> Will contain the default value.</param>
        /// <returns>Whether or not the default value is from an EnumDefaultValueAttribute.</returns>
        /// <exception cref="ArgumentException">If the supplied type is not an enum.</exception>
        public static bool GetDefaultValue<T>(out T Result)
        {
            bool result = false;
            lock (defaults)
            {
                Type thetype = typeof(T);
                if (defaults.ContainsKey(thetype))
                {
                    Result = (T)defaults[thetype];
                    result = true;
                }
                else
                {
                    if (!thetype.IsEnum)
                    {
                        throw (new ArgumentException("T must be an Enum"));
                    }
                    EnumDefaultValueAttribute[] atts = (EnumDefaultValueAttribute[])thetype.GetCustomAttributes(typeof(EnumDefaultValueAttribute), false);
                    Result = (T)Enum.GetValues(thetype).GetValue(0);
                    if (atts.Length > 0)
                    {
                        EnumDefaultValueAttribute att = atts[0];
                        if (att.value != null)
                        {
                            if (att.value is String)
                            {
                                try
                                {
                                    Result = (T)Enum.Parse(thetype, (string)att.value);
                                    result = true;
                                }
                                catch (ArgumentException err)
                                {
                                    throw (new ArgumentException(string.Format("The value ({0}) specified by the EnumDefaultValueAttribute " + "on {1} could not be parsed", att.value, thetype.ToString()), err));
                                }
                            }
                            else
                            {
                                try
                                {
                                    Result = (T)Convert.ChangeType(att.value, Enum.GetUnderlyingType(thetype)); result = true;
                                }
                                catch (OverflowException err)
                                {
                                    throw (new ArgumentException(string.Format("The value ({0}) specified by the EnumDefaultValueAttribute " + "on {1} cannot be converted to {2}", att.value, thetype.ToString(), Enum.GetUnderlyingType(thetype).ToString()), err));
                                }
                                catch (InvalidCastException err)
                                {
                                    throw (new ArgumentException(string.Format("The value ({0}) specified by the EnumDefaultValueAttribute " + "on {1} cannot be converted to {2}", att.value, thetype.ToString(), Enum.GetUnderlyingType(thetype).ToString()), err));
                                }
                            }
                        }
                        else
                        {
                            throw (new ArgumentNullException(null, string.Format("The value (<null>) specified by the EnumDefaultValueAttribute " + "on {0} could not be parsed", thetype.ToString())));
                        }
                    }
                    defaults[thetype] = Result;
                }
            }
            return (result);
        }

    }
}
