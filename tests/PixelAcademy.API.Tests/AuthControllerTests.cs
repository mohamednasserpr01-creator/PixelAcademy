using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using PixelAcademy.Application.DTOs.Auth;
using Xunit;

namespace PixelAcademy.API.Tests;

public class AuthControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _factory.SeedDatabaseAsync().GetAwaiter().GetResult();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Register_User_Successfully()
    {
        // توليد رقم هاتف عشوائي للاختبار
        var phoneNumber = $"010{new Random().Next(10000000, 99999999)}";

        var request = new RegisterRequestDto
        {
            PhoneNumber = phoneNumber,
            Username = $"user_{Guid.NewGuid():N}",
            Password = "TestPassword123!",
            FullName = "Test User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal(phoneNumber, result.User.PhoneNumber);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task Login_Successfully()
    {
        var request = new LoginRequestDto
        {
            PhoneNumber = "01011111111", // افترضنا إن ده رقم الطالب المسجل في الـ Seed
            Password = "Student123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal("01011111111", result.User.PhoneNumber);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task Me_Returns_Current_User_When_Authorized()
    {
        var loginRequest = new LoginRequestDto
        {
            PhoneNumber = "01011111111",
            Password = "Student123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(loginResult);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        var meResponse = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResult = await meResponse.Content.ReadApiResponseAsync<UserDto>();
        Assert.NotNull(meResult);
        Assert.Equal("01011111111", meResult.PhoneNumber);
        Assert.Equal("student", meResult.Username);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Me_Returns_401_Without_Token()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_Token_Works()
    {
        var loginRequest = new LoginRequestDto
        {
            PhoneNumber = "01011111111",
            Password = "Student123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrEmpty(loginResult.RefreshToken));

        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshResult = await refreshResponse.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(refreshResult);
        Assert.False(string.IsNullOrEmpty(refreshResult.AccessToken));
        Assert.False(string.IsNullOrEmpty(refreshResult.RefreshToken));
        Assert.NotEqual(loginResult.RefreshToken, refreshResult.RefreshToken);
    }

    [Fact]
    public async Task Logout_Revokes_Token()
    {
        var loginRequest = new LoginRequestDto
        {
            PhoneNumber = "01011111111",
            Password = "Student123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadApiResponseAsync<AuthResponseDto>();
        Assert.NotNull(loginResult);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Invalid_Password_Fails()
    {
        var request = new LoginRequestDto
        {
            PhoneNumber = "01011111111",
            Password = "WrongPassword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Duplicate_PhoneNumber_Fails()
    {
        var phoneNumber = $"010{new Random().Next(10000000, 99999999)}";

        var request = new RegisterRequestDto
        {
            PhoneNumber = phoneNumber,
            Username = $"user_{Guid.NewGuid():N}",
            Password = "TestPassword123!",
            FullName = "Test User"
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        request.Username = $"user2_{Guid.NewGuid():N}";
        var secondResponse = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }
}