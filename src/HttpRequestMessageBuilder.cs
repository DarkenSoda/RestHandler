using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace DarkenSoda.RestHandler
{
    [Obsolete("HttpRequestMessageBuilder is deprecated. Use The RestRequest class instead.")]
    public class HttpRequestMessageBuilder
    {
        private readonly HttpRequestMessage request;

        public HttpRequestMessageBuilder(HttpMethod method, string url)
        {
            request = new HttpRequestMessage(method, url);
        }

        #region Content
        public HttpRequestMessageBuilder WithContent(object content)
        {
            string json = JsonConvert.SerializeObject(content);
            return WithContent(json, "application/json");
        }

        public HttpRequestMessageBuilder WithContent(string content, string mediaType = "application/json")
        {
            request.Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType);
            return this;
        }
        #endregion

        #region Headers
        public HttpRequestMessageBuilder AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
            return this;
        }

        public HttpRequestMessageBuilder AddHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
                return this;

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return this;
        }
        #endregion

        public HttpRequestMessageBuilder WithAuthorization(string scheme, string parameter)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
            return this;
        }

        public HttpRequestMessage Build()
        {
            return request;
        }
    }
}
