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

1. Clone the repository:

    ```bash
    git clone https://github.com/DarkenSoda/RestHandler.git
    ```

2. Install `Newtonsoft.Json` via NuGet:

    ```bash
    Install-Package Newtonsoft.Json
    ```

    Or using .NET CLI:

    ```bash
    dotnet add package Newtonsoft.Json
    ```

3. Use the `DarkenSoda.RestHandler` namespace in your project.

## Usage

### GET Request

```csharp
using DarkenSoda.RestHandler;

MyType result = await RestRequest
    .Get("url")
    .SendAsync()
    .ParseAsAsync<MyType>();

Console.WriteLine(result);
```

### POST Request

```csharp
RequestResult result = await RestRequest
    .Post("url")
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
    .Get("url")
    .SendAsync()
    .ParseAsAsync<MyType>();

// Deferred parsing
RequestResult result = await RestRequest
    .Get("url")
    .SendAsync();

MyType data2 = result.ParseAs<MyType>();
```

### Get Raw Json String

You can get the response json as a string directly as follow:

```csharp
string json1 = result.ResponseJson;
string json2 = result.GetRawString();

string json3 = await RestRequest
    .Get("url")
    .SendAsync()
    .GetRawStringAsync();

Console.WriteLine(json1);
Console.WriteLine(json2);
Console.WriteLine(json3);
```

### Callbacks

Use OnSuccess, OnFail, and Catch for granular response handling:

```csharp
await RestRequest
    .Get("url")
    .OnSuccess(res => Console.WriteLine("Success: " + res.ResponseJson))
    .OnFail(res => Console.WriteLine("Failed: " + res.ErrorMessage))
    .Catch(ex => Console.WriteLine("Error: " + ex.Message))
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

// Timeout in seconds
HttpClientManager.SetDefaultTimeout(30);
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
var result = await ApiRequest.GetAsync<MyType>("url");
```

#### New

```csharp
var result = await RestRequest
    .Get("url")
    .SendAsync()
    .ParseAsAsync<MyType>();
```

## Roadmap

Here is the current roadmap for future updates.

- ✅ Fluent request syntax (RestRequest.Get(...).SendAsync())
- ✅ Unified RequestResult type for all outcomes
- ✅ Type-safe parsing via `ParseAs<T>()` and `ParseAsAsync<T>()`
- ⬜ Native CancellationToken support
- ⬜ **JsonName Attribute**: Attribute to change the name of a field in the object scheme

  ```csharp
    public class Student {
      [JsonName("fullName")]
      string Name { get; set; }
      
      [JsonName("age")]
      int Age { get; set; }
    }
  ```

  this makes it so you can Parse a Json with a scheme `{ "fullName": "MyName", "age": 23 }` to the Student class even if the fields are named differently.

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix (`git checkout -b feature-name`).
3. Add your feature or fix and update the [CONTRIBUTORS](CONTRIBUTORS) file with your name.
4. Commit your changes (`git commit -m 'Add feature'`).
5. Push to the branch (`git push origin feature-name`).
6. Open a pull request.

By following these steps, your contribution and name will be recognized once your pull request is merged.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
