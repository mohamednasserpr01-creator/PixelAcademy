using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Application.DTOs.Common;
using Xunit;

namespace PixelAcademy.API.Tests;

public class CoursesControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CoursesControllerTests()
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
    public async Task CreateCourse_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateCourseRequestDto
        {
            Title = "Test Course Created",
            Description = "A test course for integration testing.",
            ShortDescription = "Test course",
            Level = Domain.Enums.CourseLevel.Beginner,
            Price = 29.99m,
            Category = "Testing",
            Tags = "test,integration"
        };

        var response = await _client.PostAsJsonAsync("/api/courses", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<CourseDto>();
        Assert.NotNull(result);
        Assert.Equal("Test Course Created", result.Title);
        Assert.Equal("Testing", result.Category);
        Assert.NotEqual(Guid.Empty, result.Id);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCourseById_Returns200()
    {
        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var response = await _client.GetAsync($"/api/courses/{courseId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<CourseDetailDto>();
        Assert.NotNull(result);
        Assert.Equal("Introduction to .NET 8", result.Title);
        Assert.Equal("Programming", result.Category);
    }

    [Fact]
    public async Task GetPublishedCourses_Returns200()
    {
        var response = await _client.GetAsync("/api/courses/published?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<CourseDto>>();
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task UpdateCourse_AsOwner_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var request = new UpdateCourseRequestDto
        {
            Title = "Updated .NET 8 Course",
            Description = "Updated description.",
            Price = 59.99m
        };

        var response = await _client.PutAsJsonAsync($"/api/courses/{courseId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<CourseDto>();
        Assert.NotNull(result);
        Assert.Equal("Updated .NET 8 Course", result.Title);
        Assert.Equal(59.99m, result.Price);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DeleteCourse_AsOwner_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var response = await _client.DeleteAsync($"/api/courses/{courseId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetDeletedCourse_Returns404()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var deleteResponse = await _client.DeleteAsync($"/api/courses/{courseId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;

        var getResponse = await _client.GetAsync($"/api/courses/{courseId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateCourseRequestDto
        {
            Title = "Student Attempt Course",
            Description = "Should not be allowed.",
            Level = Domain.Enums.CourseLevel.Beginner
        };

        var response = await _client.PostAsJsonAsync("/api/courses", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCourseById_NotFound_Returns404()
    {
        var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var response = await _client.GetAsync($"/api/courses/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
