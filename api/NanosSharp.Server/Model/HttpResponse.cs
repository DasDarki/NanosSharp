using System.Net;
using System.Text.Json;

namespace NanosSharp.Server;

public record HttpResponse(HttpStatusCode Status, string DataAsString)
{
    /// <summary>
    /// Returns the data body of the HTTP response as a deserialized object of type T if the data is in JSON format.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the data into.</typeparam>
    /// <returns>The deserialized object of type T, or null if the data is empty or cannot be deserialized.</returns>
    public T? DataAs<T>() where T : class
    {
        if (string.IsNullOrEmpty(DataAsString))
            return null;

        return JsonSerializer.Deserialize<T>(DataAsString);
    }
}