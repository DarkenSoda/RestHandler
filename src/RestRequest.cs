using System.Net.Http.Headers;
using DarkenSoda.Extensions;
using DarkenSoda.Models;
using Newtonsoft.Json;
using RestHandler.Models;

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
        private Action<RequestResult, int, TimeSpan>? onRetry;
        private TimeSpan? timeout;
        private RetryPolicy? retryPolicy;

        private class RetryPolicy
        {
            public int MaxAttempts { get; set; }
            public Func<int, TimeSpan> DelayStrategy { get; set; } = delegate { return TimeSpan.Zero; };
        }

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
        public RestRequest OnException(Action<Exception> exceptionCallback)
        {
            onException = exceptionCallback;
            return this;
        }

        /// <summary>
        /// Sets a callback to be invoked on request retry.
        /// </summary>
        /// <param name="retryCallback">The callback to invoke on retry.</param>
        /// <remarks>
        /// This callback provides the current request result, the retry attempt number, and the elapsed time since the start of the request.
        /// </remarks>
        public RestRequest OnRetry(Action<RequestResult, int, TimeSpan> retryCallback)
        {
            onRetry = retryCallback;
            return this;
        }
        #endregion

        #region Retry
        /// <summary>
        /// Sets the number of retries for the request.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <remarks>
        /// This method configures the request to retry a specified number of times in case of failure.
        /// The default delay strategy is a fixed delay of 1 second between attempts.
        /// </remarks>
        public RestRequest SetRetries(int retryCount)
        {
            this.retryPolicy = new RetryPolicy
            {
                MaxAttempts = retryCount,
                DelayStrategy = Backoff.Fixed(TimeSpan.FromSeconds(1))
            };

            return this;
        }

        /// <summary>
        /// Sets the number of retries for the request with a custom delay strategy.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <param name="delayStartegy">The delay strategy function to determine the delay between attempts.</param>
        /// <remarks>
        /// This method configures the request to retry a specified number of times in case of failure.
        /// The delay strategy function is used to calculate the delay between attempts.
        /// </remarks> 
        public RestRequest SetRetries(int retryCount, Func<int, TimeSpan> delayStartegy)
        {
            this.retryPolicy = new RetryPolicy
            {
                MaxAttempts = retryCount,
                DelayStrategy = delayStartegy
            };

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
            HttpResponseMessage? response = null;
            int retryCount = retryPolicy?.MaxAttempts ?? 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int attempt = 0; attempt <= retryCount; attempt++)
            {
                if (attempt > 0)
                    await HandleRetryDelayAsync(result, attempt, stopwatch.Elapsed);

                try
                {
                    response = await ExecuteSendAsync();
                    await HandleResponseAsync(response, result);
                }
                catch (Exception ex)
                {
                    HandleException(ex, result);
                }

                response?.Dispose();

                if (result.IsSuccess())
                    break;
            }

            stopwatch.Stop();
            result.ElapsedTime = stopwatch.Elapsed;
            result.StatusCode ??= response?.StatusCode;
            return result;
        }

        #region Helper Methods
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
            var clonedRequest = CloneHttpRequestMessage(requestMessage);
            if (timeout.HasValue)
            {
                using var cts = new CancellationTokenSource(timeout.Value);
                return await HttpClientManager.Client.SendAsync(clonedRequest, cts.Token);
            }

            return await HttpClientManager.Client.SendAsync(clonedRequest);
        }

        /// <summary>
        /// Clones a given HttpRequestMessage to create a new instance with the same properties.
        /// </summary>
        /// <param name="request">The original HttpRequestMessage to clone.</param>
        /// <returns>A new HttpRequestMessage instance with the same properties as the original.</returns>
        private HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version,
                Content = request.Content == null ? null : new StreamContent(request.Content.ReadAsStream()),
            };

            // Copy headers
            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                    clone.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        /// <summary>
        /// Handles the delay before retrying the request.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <param name="attempt">The current retry attempt number.</param>
        /// <param name="elapsed">The elapsed time since the start of the request.</param>
        /// <remarks>
        /// This method updates the request result state to "Retrying" and invokes the retry callback if provided.
        /// It also calculates the delay based on the retry policy and waits for the specified duration before retrying.
        /// </remarks>
        private async Task HandleRetryDelayAsync(RequestResult result, int attempt, TimeSpan elapsed)
        {
            result.State = RequestState.Retrying;
            onRetry?.Invoke(result, attempt, elapsed);

            var delay = retryPolicy?.DelayStrategy?.Invoke(attempt) ?? TimeSpan.Zero;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay);
        }

        /// <summary>
        /// Handles the HTTP response and updates the request result accordingly.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <param name="result">The result of the request.</param>
        /// <remarks>
        /// This method checks the response status code and updates the request result state, error message, and response JSON.
        /// It also invokes the success or failure callback based on the response status.
        /// </remarks>
        private async Task HandleResponseAsync(HttpResponseMessage response, RequestResult result)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.State = RequestState.Success;
                result.ResponseJson = content;
                onSuccess?.Invoke(result);
            }
            else
            {
                result.State = RequestState.Failed;
                result.ErrorMessage = response.ReasonPhrase;
                onFailure?.Invoke(result);
            }
        }

        /// <summary>
        /// Handles exceptions that occur during the request process.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="result">The result of the request.</param>
        /// <remarks>
        /// This method updates the request result state and status code based on the exception type.
        /// It also invokes the failure or exception callback as appropriate.
        /// </remarks>
        private void HandleException(Exception ex, RequestResult result)
        {
            switch (ex)
            {
                case TaskCanceledException _ when timeout.HasValue && timeout.Value > TimeSpan.Zero:
                    result.State = RequestState.Failed;
                    result.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
                    onFailure?.Invoke(result);
                    break;

                case HttpRequestException httpEx:
                    result.State = RequestState.Failed;
                    result.StatusCode = httpEx.StatusCode ?? System.Net.HttpStatusCode.InternalServerError;
                    onFailure?.Invoke(result);
                    break;

                default:
                    result.State = RequestState.Error;
                    result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    onException?.Invoke(ex);
                    break;
            }

            result.ErrorMessage = ex.Message;
        }
        #endregion
    }
}