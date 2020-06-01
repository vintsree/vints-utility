namespace Vints.Utility
{
    using System;
    using System.Collections.Generic;

    public static class NetworkUtility
    {
        /// <summary>
        /// Retrieves a base domain name from a full domain name.
        /// For example: www.google.com produces google.com
        /// </summary>
        /// <param name="domainName">Dns Domain name as a string</param>
        /// <returns></returns>
        public static string GetBaseDomain(string domainName)
        {
            var tokens = domainName.Split('.');

            // only split 3 urls like www.google.com
            if (tokens == null || tokens.Length != 3)
                return domainName;

            var tok = new List<string>(tokens);
            var remove = tokens.Length - 2;
            tok.RemoveRange(0, remove);

            return tok[0] + "." + tok[1]; ;
        }

        /// <summary>
        /// Returns the base domain from a domain name
        /// Example: http://www.google.com returns google.com
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetBaseDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
                return GetBaseDomain(uri.DnsSafeHost);

            return uri.Host;
        }
    }
}
