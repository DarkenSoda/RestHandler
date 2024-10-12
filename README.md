# RestHandler

A simple and flexible library for handling RESTful API requests using pure C# `HttpClient` and `Newtonsoft.Json`. This library allows you to easily send `GET`, `POST`, `PUT`, and `DELETE` requests with support for headers, authentication, and custom content types.

## Jump to

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [GET Example](#sending-a-get-request)
  - [POST Example](#sending-a-post-request)
  - [Base Address and Headers](#setting-up-base-url-and-headers)
  - [Authorization](#adding-authorization)
  - [Removing Headers or Authorization](#removing-headers-or-authorization)
  - [Timeouts](#custom-timeout)
  - [Example using All Features](#example-using-all-features)
- [Contributing](#contributing)
- [License](#license)

## Features

- Ability to send requests and retrieve responses as raw JSON or deserialized objects.
- Lazy initialization of `HttpClient` to ensure memory efficiency.
- Support for adding and removing headers.
- Authorization support (e.g., Bearer tokens, custom schemes).
- Easy timeout configuration.
- Chainable `HttpRequestMessageBuilder` for constructing custom requests.

## Installation

1. Download the source code or clone the repository.

    ```bash
    git clone https://github.com/DarkenSoda/RestHandler.git
    ```

2. Ensure that you have `Newtonsoft.Json` installed in your project. You can install it via NuGet:

   ```bash
   Install-Package Newtonsoft.Json
   ```

    or using CLI

    ```bash
    dotnet add package Newtonsoft.Json
    ```

3. Add the `DarkenSoda.RestHandler` namespace to your project.

## Usage

### Sending a GET Request

You can send a `GET` request and get the response as a deserialized object or as a raw string.

```csharp
using DarkenSoda.RestHandler;
using System.Threading.Tasks;

// make sure this has the same scheme as the api
public class MyResponseType
{
    public int TestNumber { get; set; }
}

public async Task GetExample()
{
    MyResponseType response = await ApiRequest.GetAsync<MyResponseType>("https://api.example.com/data");

    // Raw JSON response
    string jsonResponse = await ApiRequest.GetRawStringAsync("https://api.example.com/data");
}
```

### Sending a POST Request

To send a `POST` request with content:

```csharp
using DarkenSoda.RestHandler;
using System.Threading.Tasks;

public async Task PostExample()
{
    MyContent requestData = new MyContent() { Name = "John", Age = 30 };

    MyResponseType response = await ApiRequest.PostAsync<MyResponseType, MyContent>("https://api.example.com/data", requestData);

    // Raw JSON response
    string jsonResponse = await ApiRequest.PostRawStringAsync<MyContent>("https://api.example.com/data", requestData);
}
```

### Setting up Base URL and Headers

Before sending any requests, you can configure the base URL and headers.

```csharp
using DarkenSoda.RestHandler;

// Set the base address for all requests
HttpClientManager.SetBaseAddress("https://api.example.com");

// Add custom headers
HttpClientManager.AddHeader("Custom-Header", "HeaderValue");
```

### Adding Authorization

If your API requires an authorization token, you can include it in the request:

```csharp
using DarkenSoda.RestHandler;

HttpClientManager.AddAuthorizationHeader("Bearer", "your-token");
```

### Removing Headers or Authorization

To remove a specific header or authorization from your request:

```csharp
HttpClientManager.RemoveHeader("Custom-Header");
// or
HttpClientManager.ClearHeaders();   // clear all headers

HttpClientManager.RemoveAuthorizationHeader();
```

### Custom Timeout

To configure the request timeout:

```csharp
HttpClientManager.SetTimeout(30); // Timeout in seconds
```

---

### Example Using All Features

This example shows how you can use everything together.

Note you can set the Headers and Authorization for the entire library using the `HttpClientManager` or send them per use as shown below:

```csharp
static async Task Main(string[] args)
{
    // Set the base address for the HttpClient
    HttpClientManager.SetBaseAddress("https://api.example.com/");

    // Define the headers, scheme, and token
    var headers = new Dictionary<string, string>
    {
        { "Custom-Header", "HeaderValue" },
        { "Another-Header", "AnotherValue" }
    };
    string token = "your-jwt-token";
    string scheme = "Bearer";

    // Example: Sending a GET request with optional headers and authorization
    string getUrl = "resource/get";
    var response = await ApiRequest.GetAsync<MyResponseType>(getUrl, scheme, token, headers);
    Console.WriteLine("GET Response: " + response);

    // Example: Sending a POST request with headers, authorization, and content
    string postUrl = "resource/post";
    MyContent postContent = new MyContent()
    {
        Name = "Example",
        Value = 123
    };
    var postResponse = await ApiRequest.PostAsync<MyResponseType2, MyContent>(postUrl, postContent, scheme, token, headers);
    Console.WriteLine("POST Response: " + postResponse);
}
```

---

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix (`git checkout -b feature-name`).
3. Add your feature or fix and update the [CONTRIBUTORS](CONTRIBUTORS) file with your name.
4. Commit your changes (`git commit -m 'Add feature'`).
5. Push to the branch (`git push origin feature-name`).
6. Open a pull request.

By following these steps, your contribution and name will be recognized once your pull request is merged.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
