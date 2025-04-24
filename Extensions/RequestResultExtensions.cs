using DarkenSoda.Models;
using Newtonsoft.Json;

namespace DarkenSoda.Extensions
{
    public static class RequestResultExtensions
    {
        /// <summary>
        /// Parses the response JSON into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to parse the JSON into.</typeparam>
        /// <param name="result">The request result containing the JSON response.</param>
        /// <returns>An object of type T if the request was successful; otherwise, default value of T.</returns>
        public static T ParseAs<T>(this RequestResult result)
        {
            if (result.State != RequestState.Success || string.IsNullOrEmpty(result.ResponseJson))
                return default!;

            try
            {
                return JsonConvert.DeserializeObject<T>(result.ResponseJson)!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not convert to {typeof(T).Name}: {ex.Message}");
                return default!;
            }
        }

        /// <summary>
        /// Asynchronously parses the response JSON into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type to parse the JSON into.</typeparam>
        /// <param name="resultTask">The task representing the request result containing the JSON response.</param>
        /// <returns>An object of type T if the request was successful; otherwise, default value of T.</returns>
        public static async Task<T> ParseAsAsync<T>(this Task<RequestResult> resultTask)
        {
            return (await resultTask).ParseAs<T>();
        }

        /// <summary>
        /// Retrieves the raw JSON string from the request result.
        /// </summary>
        /// <param name="result">The request result containing the JSON response.</param>
        /// <returns>The response JSON if available; otherwise, an empty string.</returns>
        public static string GetRawString(this RequestResult result)
        {
            return result.ResponseJson ?? string.Empty;
        }

        /// <summary>
        /// Asynchronously retrieves the raw JSON string from the request result.
        /// </summary>
        /// <param name="resultTask">The task representing the request result containing the JSON response.</param>
        /// <returns>The response JSON if available; otherwise, an empty string.</returns>
        public static async Task<string> GetRawStringAsync(this Task<RequestResult> resultTask)
        {
            return (await resultTask).GetRawString();
        }

        /// <summary>
        /// Checks if the request was successful.
        /// </summary>
        /// <param name="result">The request result to check.</param>
        public static bool IsSuccess(this RequestResult result)
        {
            return result.State == RequestState.Success;
        }
    }
}
