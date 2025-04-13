namespace DarkenSoda.Models
{
    /// <summary>
    /// Represents the state of a request.
    /// </summary> 
    /// <remarks>
    /// The state of a request can be one of the following:
    /// <list type="bullet">
    /// <item><description>Sending: The request is being sent.</description></item>
    /// <item><description>Success: The request was successful.</description></item>
    /// <item><description>Failed: The request failed.</description></item>
    /// <item><description>Error: An error occurred while processing the request.</description></item>
    /// </list>
    /// </remarks>
    public enum RequestState
    {
        Sending,
        Success,
        Failed,
        Error,
    }
}
