using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DarkenSoda.RestHandler
{
    public static class ApiRequest
    {
        /// <summary>
        /// Constructs an HttpRequestMessage object with specified
        /// method, URL, content, authorization token, headers, and scheme.
        /// </summary>
        /// <param name="HttpMethod">Method used in the request, such as GET, POST, PUT, DELETE, etc.</param>
        /// <param name="url">The link for the API request</param>
        /// <param name="content">The Content to be send to the request. Used in POST & PUT only.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns an HttpRequestMessage object.</returns>
        public static HttpRequestMessage CreateRequest(HttpMethod method, string url, object content = null, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            var builder = new HttpRequestMessageBuilder(method, url);

            if (!string.IsNullOrEmpty(token))
            {
                builder.WithAuthorization(scheme, token);
            }

            if (content != null)
            {
                builder.WithContent(content);
            }

            return builder.AddHeaders(headers).Build();
        }

        /// <summary>
        /// Sends an HttpRequestMessage object and returns the response as a string.
        /// Used to retrieve the response body only.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object to be sent.</param>
        /// <returns>Returns the response as a string.</returns>
        public static async Task<string> SendAndGetStringAsync(HttpRequestMessage request)
        {
            var response = await HttpClientManager.Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Sends an HttpRequestMessage object and returns the response as an HttpResponseMessage.
        /// Used to retrieve the response status code, headers, body, etc.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object to be sent.</param>
        /// <returns>Returns the response as an HttpResponseMessage.</returns>
        public static async Task<HttpResponseMessage> SendAndGetResponseAsync(HttpRequestMessage request)
        {
            return await HttpClientManager.Client.SendAsync(request);
        }

        #region GET
        /// <summary>
        /// Sends a GET request to the specified URL and returns the response as a deserialized object of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type of the object to be deserialized.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a deserialized object of type TResponse.</returns>
        public static async Task<TResponse> GetAsync<TResponse>(string url, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            string json = await GetRawStringAsync(url, scheme, token, headers);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        /// <summary>
        /// Sends a GET request to the specified URL and returns the response as a string.
        /// </summary>
        /// <param name="url">The link for the API request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a string.</returns>
        public static async Task<string> GetRawStringAsync(string url, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            HttpRequestMessage request = CreateRequest(HttpMethod.Get, url, null, scheme, token, headers);
            return await SendAndGetStringAsync(request);
        }
        #endregion

        #region POST
        /// <summary>
        /// Sends a POST request to the specified URL and returns the response as a deserialized object of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type of the object to be deserialized.</typeparam>
        /// <typeparam name="TContent">The type of the content to be sent.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="content">The Content to be send to the request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a deserialized object of type TResponse.</returns>
        public static async Task<TResponse> PostAsync<TResponse, TContent>(string url, TContent content, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            string json = await PostRawStringAsync(url, content, scheme, token, headers);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        /// <summary>
        /// Sends a POST request to the specified URL and returns the response as a string.
        /// </summary>
        /// <typeparam name="TContent">The type of the content to be sent.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="content">The Content to be send to the request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a string.</returns>
        public static async Task<string> PostRawStringAsync<TContent>(string url, TContent content, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            HttpRequestMessage request = CreateRequest(HttpMethod.Post, url, content, scheme, token, headers);
            return await SendAndGetStringAsync(request);
        }
        #endregion

        #region PUT
        /// <summary>
        /// Sends a PUT request to the specified URL and returns the response as a deserialized object of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type of the object to be deserialized.</typeparam>
        /// <typeparam name="TContent">The type of the content to be sent.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="content">The Content to be send to the request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a deserialized object of type TResponse.</returns>
        public static async Task<TResponse> PutAsync<TResponse, TContent>(string url, TContent content, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            string json = await PutRawStringAsync(url, content, scheme, token, headers);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        /// <summary>
        /// Sends a PUT request to the specified URL and returns the response as a string.
        /// </summary>
        /// <typeparam name="TContent">The type of the content to be sent.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="content">The Content to be send to the request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a string.</returns>
        public static async Task<string> PutRawStringAsync<TContent>(string url, TContent content, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            HttpRequestMessage request = CreateRequest(HttpMethod.Put, url, content, scheme, token, headers);
            return await SendAndGetStringAsync(request);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Sends a DELETE request to the specified URL and returns the response as a deserialized object of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type of the object to be deserialized.</typeparam>
        /// <param name="url">The link for the API request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a deserialized object of type TResponse.</returns>
        public static async Task<TResponse> DeleteAsync<TResponse>(string url, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            string json = await DeleteRawStringAsync(url, scheme, token, headers);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        /// <summary>
        /// Sends a DELETE request to the specified URL and returns the response as a string.    
        /// </summary>
        /// <param name="url">The link for the API request.</param>
        /// <param name="scheme">Represents the authentication scheme to be used for the request.</param>
        /// <param name="token">The Authorization Token.</param>
        /// <param name="headers">Additional header data.</param>
        /// <returns>Returns the response as a string.</returns>
        public static async Task<string> DeleteRawStringAsync(string url, string scheme = "Bearer", string token = null, Dictionary<string, string> headers = null)
        {
            HttpRequestMessage request = CreateRequest(HttpMethod.Delete, url, null, scheme, token, headers);
            return await SendAndGetStringAsync(request);
        }
        #endregion
    }
}
