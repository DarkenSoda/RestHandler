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
        public static void AddDefaultHeader(string key, string value)
        {
            Client.DefaultRequestHeaders.Add(key, value);
        }

        public static void AddDefaultHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                Client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public static void RemoveDefaultHeader(string key)
        {
            Client.DefaultRequestHeaders.Remove(key);
        }

        public static void RemoveDefaultHeaders(List<string> keys)
        {
            if (keys == null)
                return;

            foreach (var key in keys)
            {
                Client.DefaultRequestHeaders.Remove(key);
            }
        }

        public static void ClearDefaultHeaders()
        {
            Client.DefaultRequestHeaders.Clear();
        }
        #endregion

        #region Authorization
        public static void AddDefaultAuthorizationHeader(string scheme, string parameter)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }

        public static void RemoveDefaultAuthorizationHeader()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
        #endregion

        #region Timeout
        public static void SetDefaultTimeout(TimeSpan timeout)
        {
            Client.Timeout = timeout;
        }

        public static void SetDefaultTimeout(uint seconds)
        {
            Client.Timeout = TimeSpan.FromSeconds(seconds);
        }
        #endregion
    }
}
