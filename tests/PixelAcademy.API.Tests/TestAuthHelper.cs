using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.API.Tests;

public static class TestAuthHelper
{
    public static async Task<AuthResponseDto> LoginAsync(HttpClient client, string email, string password)
    {
        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadApiResponseAsync<AuthResponseDto>();
        if (result == null)
            throw new InvalidOperationException("Login returned null result");
        return result;
    }
}
