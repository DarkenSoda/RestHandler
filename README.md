# RestHandler

RestHandler is a lightweight, developer-friendly C# library for crafting and sending RESTful HTTP requests with ease. Built on `HttpClient` and `Newtonsoft.Json`, it features a fluent, expressive API that keeps your code clean, readable, and frustration-free. Whether you're calling APIs in a side project or integrating with enterprise systems, RestHandler aims to make networking in C# feel a little more joyful.

## Jump to

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [GET Request](#get-request)
  - [POST Request](#post-request)
  - [Response Parsing](#response-parsing)
  - [Callbacks](#callbacks)
  - [Setting Timeout](#setting-timeout)
  - [Setting Retry Policy](#setting-retry-policy)
  - [Adding Query Parameters](#adding-query-parameters)
  - [Setting Global Parameter](#setting-global-parameters)
- [Migrating from Old API](#migrating-from-old-api)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

## Features

- Fluent API for clean and chainable request creation
- Support for `GET`, `POST`, `PUT`, and `DELETE`
- Global base address, headers, and authorization
- Response parsing into any type with `ParseAs<T>()` or `ParseAsAsync<T>()`
- Built-in success/fail/error callbacks
- Raw JSON access and error state detection

## Installation

### Install via NuGet

```bash
dotnet add package RestHandler
```

Or using the NuGet Package Manager:

```bash
Install-Package RestHandler
```

## Usage

### GET Request

```csharp
using DarkenSoda.RestHandler;

MyType result = await RestRequest
    .Get("URL")
    .SendAsync()
    .ParseAsAsync<MyType>();

Console.WriteLine(result);
```

### POST Request

```csharp
RequestResult result = await RestRequest
    .Post("URL")
    .WithContent(new { name = "test", age = 10 })
    .WithAuthorization("Bearer", "your-token")
    .SendAsync();

Console.WriteLine($"Status: {result.State}");
```

### Response Parsing

You can parse the result directly during or after the request:

```csharp
// Direct parsing
MyType data = await RestRequest
    .Get("URL")
    .SendAsync()
    .ParseAsAsync<MyType>();

// Deferred parsing
RequestResult result = await RestRequest
    .Get("URL")
    .SendAsync();

MyType data2 = result.ParseAs<MyType>();
```

### Get Raw Json String

You can get the response json as a string directly as follow:

```csharp
string json1 = result.ResponseJson;
string json2 = result.GetRawString();

string json3 = await RestRequest
    .Get("URL")
    .SendAsync()
    .GetRawStringAsync();

Console.WriteLine(json1);
Console.WriteLine(json2);
Console.WriteLine(json3);
```

### Callbacks

Use OnSuccess, OnFail, OnException and OnRetry for granular response handling:

```csharp
await RestRequest
    .Get("URL")
    .OnSuccess(res => Console.WriteLine("Success: " + res.ResponseJson))
    .OnFail(res => Console.WriteLine("Failed: " + res.ErrorMessage))
    .OnException(ex => Console.WriteLine("Error: " + ex.Message))
    .OnRetry((response, retryCount, elapsedTime) =>
    {
        Console.WriteLine($"Retrying... {retryCount} - Elapsed Time: {elapsedTime.TotalSeconds:f2}s");
    }) // Only works if you set the retries
    .SendAsync();

```

### Setting Timeout

```csharp
// Setting Default Timeout for all requests
HttpClientManager.SetDefaultTimeout(30); // in seconds
// or using a TimeSpan
HttpClientManager.SetDefaultTimeout(TimeSpan.FromSeconds(30));

// Setting Timeout per request
RestRequest
    .Get("URL")
    .SetTimeout(5); // in seconds
// or using a TimeSpan
RestRequest
    .Get("URL")
    .SetTimeout(TimeSpan.FromSeconds(5));

```

### Setting Retry Policy

```csharp
await RestRequest
    .Get("URL")
    .SetRetries(3) // 3 retries
    .OnRetry((response, retryCount, elapsedTime) =>
    {
        Console.WriteLine($"Retrying... {retryCount} - Elapsed Time: {elapsedTime.TotalSeconds:f2}s");
    })
    .SendAsync();

// You can set a Backoff strategy as well
// Check Backoff class for more options:
// - Fixed: Fixed backoff with a fixed delay
// - Linear: Linear backoff with an increment value and a maximum delay
// - Exponential: Exponential backoff with a base delay and a maximum delay
// - Jitter: Random backoff with a maximum delay
await RestRequest
    .Get("URL")
    .SetRetries(3, Backoff.Fixed(TimeSpan.FromSeconds(1))) // 3 retries with fixed backoff of 1 second
    .OnRetry((response, retryCount, elapsedTime) =>
    {
        Console.WriteLine($"Retrying... {retryCount} - Elapsed Time: {elapsedTime.TotalSeconds:f2}s");
    })
    .SendAsync(); 
```

### Adding Query Parameters

```csharp
// specifying query parameters in the URL
await RestRequest
    .Get("URL?key=val")
    .SendAsync();

// sending only 1 query parameter
await RestRequest
    .Get("URL")
    .AddQueryParameters("key", "val")
    .SendAsync();

// sending a dictionary of query parameters
await RestRequest
    .Get("URL")
    .AddQueryParameters(new Dictionary<string, string>()
    {
        { "key1", "val1" },
        { "key2", "val2" },
        { "key3", "val3" }
    })
    .SendAsync();

// sending an object
var obj = new { key1 = val1, key2 = val2, key3 = val3 };
await RestRequest
    .Get("URL")
    .AddQueryParameters(obj)
    .SendAsync();
```

### Setting Global Parameters

Use HttpClientManager to configure defaults:

```csharp
// Must be set before sending any requests
HttpClientManager.SetBaseAddress("https://api.example.com/");

// Default headers
HttpClientManager.AddDefaultHeader("myKey", "myValue");

// Add multiple headers
HttpClientManager.AddDefaultHeaders(new Dictionary<string, string>
{
    { "key1", "value1" },
    { "key2", "value2" }
});

// Authorization
HttpClientManager.AddDefaultAuthorizationHeader("Bearer", "your-token");
```

You can remove or clear them too:

```csharp
HttpClientManager.RemoveBaseAddress();  // Must be set before sending any requests

HttpClientManager.RemoveDefaultHeader("myKey");
HttpClientManager.RemoveDefaultHeaders(new List<string>() { "key1", "key2" });
HttpClientManager.ClearDefaultHeaders();

HttpClientManager.RemoveDefaultAuthorizationHeader();
```

### Migrating from Old API

Old ApiRequest methods are now deprecated. Use RestRequest.Get(...).SendAsync() instead for all new development. Here's how to migrate:

#### Old

```csharp
var result = await ApiRequest.GetAsync<MyType>("URL");
```

#### New

```csharp
var result = await RestRequest
    .Get("URL")
    .SendAsync()
    .ParseAsAsync<MyType>();
```

## Roadmap

Here is the current roadmap for future updates.

- ✅ Fluent request syntax (RestRequest.Get(...).SendAsync())
- ✅ Unified RequestResult type for all outcomes
- ✅ Type-safe parsing via `ParseAs<T>()` and `ParseAsAsync<T>()`
- ✅ Global and per Request Timeout
- ✅ Request Retrying with Backoff
- ✅ Reusable Request Templates
- ✅ Support Query Parameter
- ⬜ Mock Requests for Testing

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix (`git checkout -b feature-name`).
3. Add your feature or fix and update the [CONTRIBUTORS](CONTRIBUTORS.md) file with your name.
4. Commit your changes (`git commit -m 'Add feature'`).
5. Push to the branch (`git push origin feature-name`).
6. Open a pull request.

By following these steps, your contribution and name will be recognized once your pull request is merged.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
