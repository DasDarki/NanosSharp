#nullable enable

namespace NanosSharp.Enums;

/// <summary>HTTP defines a set of request methods to indicate the desired action to be performed for a given resource. Read more here.</summary>
public enum HTTPMethod
{
    /// <summary>The GET method requests a representation of the specified resource. Requests using GET should only retrieve data.</summary>
    GET = 0,
    /// <summary>The POST method submits an entity to the specified resource, often causing a change in state or side effects on the server.</summary>
    POST = 1,
    /// <summary>The PUT method replaces all current representations of the target resource with the request payload.</summary>
    PUT = 2,
    /// <summary>The HEAD method asks for a response identical to a GET request, but without the response body.</summary>
    HEAD = 3,
    /// <summary>The DELETE method deletes the specified resource.</summary>
    DELETE = 4,
    /// <summary>The PATCH method applies partial modifications to a resource.</summary>
    PATCH = 5,
    /// <summary>The OPTIONS method describes the communication options for the target resource.</summary>
    OPTIONS = 6,
}
