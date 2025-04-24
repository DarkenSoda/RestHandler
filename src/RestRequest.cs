using System.Net.Http.Headers;
using DarkenSoda.Models;
using Newtonsoft.Json;

namespace DarkenSoda.RestHandler
{
    /// <summary>
    /// Represents a REST request that can be sent to a server.
    /// </summary>
    /// <remarks>
    /// This class provides methods to create and configure HTTP requests, including setting headers, content, and authorization.
    /// It also allows for handling success, failure, and exceptions through callbacks.
    /// </remarks>
    public class RestRequest
    {
        private HttpRequestMessage requestMessage;
        private Action<RequestResult>? onSuccess;
        private Action<RequestResult>? onFailure;
        private Action<Exception>? onException;
        private TimeSpan? timeout;

        private RestRequest(HttpMethod method, string url)
        {
            requestMessage = new HttpRequestMessage(method, url);
        }

        private static RestRequest Create(HttpMethod method, string url)
        {
            return new RestRequest(method, url);
        }

        #region Request Methods
        /// <summary>
        /// Creates a new GET request.
        /// </summary>
        public static RestRequest Get(string url)
        {
            return RestRequest.Create(HttpMethod.Get, url);
        }

        /// <summary>
        /// Creates a new POST request.
        /// </summary>
        public static RestRequest Post(string url)
        {
            return RestRequest.Create(HttpMethod.Post, url);
        }

        /// <summary>
        /// Creates a new PUT request.
        /// </summary>
        public static RestRequest Put(string url)
        {
            return RestRequest.Create(HttpMethod.Put, url);
        }

        /// <summary>
        /// Creates a new DELETE request.
        /// </summary>
        public static RestRequest Delete(string url)
        {
            return RestRequest.Create(HttpMethod.Delete, url);
        }
        #endregion

        #region Headers
        /// <summary>
        /// Adds a single header to the request.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value.</param>
        public RestRequest AddHeader(string key, string value)
        {
            requestMessage.Headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds multiple headers to the request.
        /// </summary>
        /// <param name="headers">A dictionary of headers to add.</param>
        public RestRequest AddHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
                return this;

            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            return this;
        }
        #endregion

        #region Content
        /// <summary>
        /// Adds content to the request as JSON.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <remarks>
        /// This method serializes the content to JSON format and sets the appropriate media type.
        /// </remarks>
        public RestRequest WithContent(object content)
        {
            string json = JsonConvert.SerializeObject(content);
            return WithContent(json, "application/json");
        }

        /// <summary>
        /// Adds content to the request with a specified media type.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <param name="mediaType">The media type of the content.</param>
        /// <remarks>
        /// This method sets the content of the request with the specified media type.
        /// </remarks>
        public RestRequest WithContent(string content, string mediaType = "application/json")
        {
            requestMessage.Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType);
            return this;
        }
        #endregion

        #region Authorization
        /// <summary>
        /// Adds an authorization header to the request.
        /// </summary>
        /// <param name="scheme">The authentication scheme (e.g., "Bearer").</param>
        /// <param name="parameter">The authentication parameter (e.g., token).</param>
        public RestRequest WithAuthorization(string scheme, string parameter)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
            return this;
        }
        #endregion

        #region Timeout
        /// <summary>
        /// Sets the timeout for the request.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        public RestRequest SetTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the timeout for the request in seconds.
        /// </summary>
        /// <param name="seconds">The timeout duration in seconds.</param>
        public RestRequest SetTimeout(uint seconds)
        {
            return SetTimeout(TimeSpan.FromSeconds(seconds));
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Sets a callback to be invoked on successful request completion.
        /// </summary>
        /// <param name="successCallback">The callback to invoke on success.</param>
        public RestRequest OnSuccess(Action<RequestResult> successCallback)
        {
            onSuccess = successCallback;
            return this;
        }

        /// <summary>
        /// Sets a callback to be invoked on request failure.
        /// </summary>
        /// <param name="failureCallback">The callback to invoke on failure.</param>
        public RestRequest OnFail(Action<RequestResult> failureCallback)
        {
            onFailure = failureCallback;
            return this;
        }

        /// <summary>
        /// Sets a callback to be invoked on request exception.
        /// </summary>
        /// <param name="exceptionCallback">The callback to invoke on exception.</param>
        public RestRequest Catch(Action<Exception> exceptionCallback)
        {
            onException = exceptionCallback;
            return this;
        }
        #endregion

        /// <summary>
        /// Sends the request asynchronously and returns the result.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the request result.</returns>
        /// <remarks>
        /// This method sends the request and handles the response, invoking the appropriate callbacks based on the result.
        /// </remarks>
        public async Task<RequestResult> SendAsync()
        {
            var result = new RequestResult { State = RequestState.Sending };

            try
            {
                HttpResponseMessage response = await ExecuteSendAsync();
                if (response.IsSuccessStatusCode)
                {
                    result.State = RequestState.Success;
                    result.ResponseJson = await response.Content.ReadAsStringAsync();
                    onSuccess?.Invoke(result);
                }
                else
                {
                    result.State = RequestState.Failed;
                    result.ErrorMessage = response.ReasonPhrase;
                    onFailure?.Invoke(result);
                }

                response.Dispose();
            }
            catch (Exception ex)
            {
                result.State = RequestState.Error;
                result.ErrorMessage = ex.Message;
                onException?.Invoke(ex);
            }

            return result;
        }

        /// <summary>
        /// Sends the request asynchronously
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the Http response.</returns>
        /// <remarks>
        /// This method sends the request and returns the raw HTTP response without invoking any callbacks.
        /// if there is a timeout set and uses a CancellationTokenSource to cancel the request if it exceeds the specified timeout.
        /// </remarks>
        private async Task<HttpResponseMessage> ExecuteSendAsync()
        {
            HttpResponseMessage response;
            if (timeout.HasValue)
            {
                using var cts = new CancellationTokenSource(timeout.Value);
                response = await HttpClientManager.Client.SendAsync(requestMessage, cts.Token);
            }
            else
            {
                response = await HttpClientManager.Client.SendAsync(requestMessage);
            }

            return response;
        }
    }
}