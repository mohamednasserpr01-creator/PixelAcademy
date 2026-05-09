using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.DTOs.WatchSessions;
using Xunit;

namespace PixelAcademy.API.Tests;

public class DiagnosticsTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DiagnosticsTests()
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
    public async Task Diagnose_UpdateProgress_500()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start session
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var startResponse = await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        // Update progress
        var updateRequest = new UpdateWatchProgressRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CurrentPositionSeconds = 900,
            DurationWatchedSeconds = 900
        };
        var updateResponse = await _client.PostAsJsonAsync("/api/watchsessions/update-progress", updateRequest);

        if (updateResponse.StatusCode != HttpStatusCode.OK)
        {
            var errorBody = await updateResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Expected OK but got {updateResponse.StatusCode}. Body: {errorBody}");
        }

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
