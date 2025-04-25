using System.Net;

namespace DarkenSoda.Models
{
    /// <summary>
    /// Represents the result of a request.
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// The state of the request.
        /// </summary>
        /// <value>
        /// Sending, Success, Failed, Error, Retrying.
        /// </value>
        public RequestState State { get; set; }

        /// <summary>
        /// The HTTP status code returned from the request.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// The error message if the request failed or encountered an error.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The JSON response from the request.
        /// </summary>
        public string? ResponseJson { get; set; }

        /// <summary>
        /// The time taken to complete the request.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        override public string ToString()
        {
            return $"RequestResult: {{State: {State}, StatusCode: {StatusCode}, ErrorMessage: {ErrorMessage}, ResponseJson: {ResponseJson}, ElapsedTime: {ElapsedTime.TotalSeconds:f2}s}}";
        }
    }
}
