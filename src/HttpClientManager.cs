using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DarkenSoda.RestHandler
{
    public static class HttpClientManager
    {
        // Lazy initialization of HttpClient so it doesn't allocate memory until it's used
        private static readonly Lazy<HttpClient> lazyClient = new Lazy<HttpClient>(() => new HttpClient());
        internal static HttpClient Client => lazyClient.Value;

        #region Base Address
        public static void SetBaseAddress(string baseAddress)
        {
            Client.BaseAddress = new Uri(baseAddress);
        }

        public static void RemoveBaseAddress()
        {
            Client.BaseAddress = null;
        }
        #endregion

        #region Headers
        public static void AddHeader(string key, string value)
        {
            Client.DefaultRequestHeaders.Add(key, value);
        }

        public static void AddHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                Client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public static void RemoveHeader(string key)
        {
            Client.DefaultRequestHeaders.Remove(key);
        }

        public static void ClearHeaders()
        {
            Client.DefaultRequestHeaders.Clear();
        }
        #endregion

        #region Authorization
        public static void AddAuthorizationHeader(string scheme, string parameter)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }

        public static void RemoveAuthorizationHeader()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
        #endregion

        #region Timeout
        public static void SetTimeout(int seconds)
        {
            Client.Timeout = TimeSpan.FromSeconds(seconds);
        }
        #endregion
    }
}
