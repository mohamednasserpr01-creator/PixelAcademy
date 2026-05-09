using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using PixelAcademy.API.Models;

namespace PixelAcademy.API.Tests;

public static class ApiResponseExtensions
{
    public static async Task<T?> ReadApiResponseAsync<T>(this HttpContent content)
    {
        var jsonString = await content.ReadAsStringAsync();
        var jsonNode = JsonNode.Parse(jsonString);
        if (jsonNode == null) return default;

        var dataNode = jsonNode["data"];
        if (dataNode == null) return default;

        return dataNode.Deserialize<T>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<ApiResponse?> ReadApiResponseRawAsync(this HttpContent content)
    {
        return await content.ReadFromJsonAsync<ApiResponse>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });
    }

    public static async Task<T?> ReadApiResponseAsync<T>(this HttpResponseMessage response)
    {
        if (response.Content == null) return default;
        return await response.Content.ReadApiResponseAsync<T>();
    }

    public static async Task<ApiResponse?> ReadApiResponseRawAsync(this HttpResponseMessage response)
    {
        if (response.Content == null) return null;
        return await response.Content.ReadApiResponseRawAsync();
    }
}
