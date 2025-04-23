using System.Net;
using DarkenSoda.Extensions;
using DarkenSoda.Models;
using DarkenSoda.RestHandler;

namespace DarkenSoda.Samples
{
    public sealed class SampleProgram
    {
        public static async Task RunSamplesAsync()
        {
            //* Setting Default Parameters
            // be careful with this, it will set the default parameters for all requests

            //! Base Address should only be set before sending any requests, otherwise it will throw an exception
            // HttpClientManager.SetBaseAddress("https://api.thecatapi.com/v1/");
            // HttpClientManager.RemoveBaseAddress();

            List<CatImages> images = await RestRequest
                .Get("https://api.thecatapi.com/v1/images/search")
                .SendAsync()
                .ParseAsAsync<List<CatImages>>();   // to parse the response directly during the request

            Console.WriteLine(images[0].ToString());

            HttpClientManager.AddDefaultHeader("myKey", "myValue");
            HttpClientManager.RemoveDefaultHeader("myKey");

            HttpClientManager.AddDefaultHeaders(new Dictionary<string, string>()
            {
                { "myKey", "myValue" },
                { "myKey2", "myValue2" }
            });
            HttpClientManager.RemoveDefaultHeaders(new List<string>() { "myKey", "myKey2" });
            HttpClientManager.ClearDefaultHeaders();

            HttpClientManager.AddDefaultAuthorizationHeader("Bearer", "myToken");
            HttpClientManager.RemoveDefaultAuthorizationHeader();

            // HttpClientManager.SetDefaultTimeout(30); // in seconds

            Console.WriteLine("\n--------------------------------------------------\n");

            //* OLD WAY (deprecated)
            string json = await ApiRequest.GetRawStringAsync("https://api.thecatapi.com/v1/images/search");
            Console.WriteLine(json);

            Console.WriteLine("\n--------------------------------------------------\n");

            //* NEW WAY
            RequestResult result = await RestRequest
                .Get("https://api.thecatapi.com/v1/images/search")
                .OnSuccess((response) => { Console.WriteLine("Succes: " + response.ResponseJson); })
                .OnFail((response) => { Console.WriteLine("Fail: " + response.ErrorMessage); })
                .Catch((exception) => { Console.WriteLine("Error: " + exception.Message); })
                .SendAsync();
            // .ParseAsAsync<List<CatImages>>(); // to parse the response directly during the request

            Console.WriteLine(result);

            List<CatImages> img = result.ParseAs<List<CatImages>>(); // to parse the response after the request is completed
            Console.WriteLine("Parsed: " + img[0].ToString());

            Console.WriteLine("\n--------------------------------------------------\n");

            //* New POST example
            RequestResult r2 = await RestRequest        // Unauthorized request
                .Post("https://api.thecatapi.com/v1/images/upload/")
                .WithContent(new { name = "test", age = 10 })
                .WithAuthorization("Bearer", "token")
                .OnFail((response) => { Console.WriteLine("R2 Failed: " + response.ErrorMessage); })
                .SendAsync();

            Console.WriteLine($"R2 Status: {r2.State}");

            Console.WriteLine("\n--------------------------------------------------\n");

            //* Raw Json string example
            string json1 = result.ResponseJson!;
            string json2 = result.GetRawString();

            string json3 = await RestRequest
                .Get("https://api.thecatapi.com/v1/images/search")
                .SendAsync()
                .GetRawStringAsync();

            Console.WriteLine(json1);
            Console.WriteLine(json2);
            Console.WriteLine(json3);
        }
    }

    internal sealed class CatImages
    {
        public string? id { get; set; }
        public string? url { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        override public string ToString()
        {
            return $"ID: {id}, URL: {url}, Width: {width}, Height: {height}";
        }
    }
}
