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
        /// Sending, Success, Failed, Error.
        /// </value>
        public RequestState State { get; set; }

        /// <summary>
        /// The error message if the request failed or encountered an error.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The JSON response from the request.
        /// </summary>
        public string? ResponseJson { get; set; }

        override public string ToString()
        {
            return $"RequestResult: {{State: {State}, ErrorMessage: {ErrorMessage}, ResponseJson: {ResponseJson}}}";
        }
    }
}
