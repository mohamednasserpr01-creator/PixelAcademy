using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Application.DTOs.Auth;
using Xunit;

namespace PixelAcademy.API.Tests;

public class LecturesControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public LecturesControllerTests()
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
    public async Task CreateLecture_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var request = new CreateLectureRequestDto
        {
            Title = "New Integration Test Lecture",
            Description = "A lecture created during integration testing.",
            OrderIndex = 3,
            DurationMinutes = 30,
            IsPreview = false,
            VideoUrl = "https://example.com/video.mp4"
        };

        var response = await _client.PostAsJsonAsync($"/api/courses/{courseId}/lectures", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<LectureDto>();
        Assert.NotNull(result);
        Assert.Equal("New Integration Test Lecture", result.Title);
        Assert.Equal(3, result.OrderIndex);
        Assert.Equal(30, result.DurationMinutes);
        Assert.False(result.IsPreview);
        Assert.Equal(courseId, result.CourseId);
        Assert.NotEqual(Guid.Empty, result.Id);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateLecture_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var request = new CreateLectureRequestDto
        {
            Title = "Student Lecture Attempt",
            Description = "Should fail.",
            OrderIndex = 99,
            DurationMinutes = 10,
            IsPreview = true
        };

        var response = await _client.PostAsJsonAsync($"/api/courses/{courseId}/lectures", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetLecturesByCourse_Returns200_OrderedByIndex()
    {
        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var response = await _client.GetAsync($"/api/courses/{courseId}/lectures");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<LectureDto>>();
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);

        // Verify ordering by OrderIndex
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].OrderIndex <= result[i + 1].OrderIndex,
                $"Lectures should be ordered by OrderIndex. Item[{i}].OrderIndex={result[i].OrderIndex} > Item[{i+1}].OrderIndex={result[i+1].OrderIndex}");
        }
    }

    [Fact]
    public async Task GetLectureById_Returns200()
    {
        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var response = await _client.GetAsync($"/api/courses/{courseId}/lectures/{lectureId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<LectureDto>();
        Assert.NotNull(result);
        Assert.Equal("Getting Started with .NET 8", result.Title);
        Assert.Equal(1, result.OrderIndex);
        Assert.True(result.IsPreview);
    }

    [Fact]
    public async Task UpdateLecture_AsInstructor_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var request = new UpdateLectureRequestDto
        {
            Title = "Updated Lecture Title",
            Description = "Updated description.",
            OrderIndex = 5,
            DurationMinutes = 50,
            IsPreview = false,
            VideoUrl = "https://example.com/updated.mp4"
        };

        var response = await _client.PutAsJsonAsync($"/api/courses/{courseId}/lectures/{lectureId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<LectureDto>();
        Assert.NotNull(result);
        Assert.Equal("Updated Lecture Title", result.Title);
        Assert.Equal(5, result.OrderIndex);
        Assert.Equal(50, result.DurationMinutes);
        Assert.False(result.IsPreview);
        Assert.Equal("https://example.com/updated.mp4", result.VideoUrl);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DeleteLecture_AsInstructor_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var response = await _client.DeleteAsync($"/api/courses/{courseId}/lectures/{lectureId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetDeletedLecture_Returns404()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var deleteResponse = await _client.DeleteAsync($"/api/courses/{courseId}/lectures/{lectureId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;

        var getResponse = await _client.GetAsync($"/api/courses/{courseId}/lectures/{lectureId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetLectureById_NotFound_Returns404()
    {
        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var response = await _client.GetAsync($"/api/courses/{courseId}/lectures/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
