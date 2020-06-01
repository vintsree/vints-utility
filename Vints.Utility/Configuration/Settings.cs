namespace Vints.Utility
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;
    public static class Settings
    {
        /// <summary>
        ///  Reads the key from <see cref="ConfigurationManager.AppSettings"/> and send the value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <param name="defaultValue"></param>
        /// <param name="compare"></param>
        /// <returns>T</returns>
        public static T ReadConfigAppSetting<T>(string searchKey, T defaultValue, StringComparison compare = StringComparison.Ordinal)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Any(key => string.Compare(key, searchKey, compare) == 0))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                if (converter != null)
                {
                    defaultValue = (T)converter.ConvertFromString(ConfigurationManager.AppSettings.GetValues(searchKey).First());
                }
            }
            return defaultValue;
        }
    }
}
