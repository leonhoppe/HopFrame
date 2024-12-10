using System.Web;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Tests.Api.Extensions;

internal static class HttpContextExtensions {
    /// <summary>Extracts the partial cookie value from the header section.</summary>
    /// <param name="headers"><inheritdoc cref="IHeaderDictionary" path="/summary"/></param>
    /// <param name="key">The key for identifying the cookie.</param>
    /// <returns>The value of the cookie.</returns>
    public static string FindCookie(this IHeaderDictionary headers, string key)
    {
        string headerKey = $"{key}=";
        var cookies = headers.Values
            .SelectMany(h => h)
            .Where(header => header.StartsWith(headerKey))
            .Select(header => header.Substring(headerKey.Length).Split(';').First())
            .ToArray();

        //Note: cookie values in a header are encoded like a uri parameter value.
        var value = cookies.LastOrDefault();//and the last set value, is the relevant one.
        if (string.IsNullOrEmpty(value))
            return null;

        //That's why we should decode that last value, before we return it.
        var decoded = HttpUtility.UrlDecode(value);
        return decoded; 
    }
}