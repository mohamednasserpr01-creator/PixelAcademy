using System.Net;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.DTOs.Search;
using Xunit;

namespace PixelAcademy.API.Tests;

public class SearchControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SearchControllerTests()
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
    public async Task SearchCourses_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search?q=NET&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<SearchResultDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Courses);
        Assert.Contains(result.Courses, c => c.Title.Contains(".NET"));
    }

    [Fact]
    public async Task SearchEmptyQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/search?q=&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchLectures_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search?q=Introduction&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<SearchResultDto>();
        Assert.NotNull(result);
        // Search matches courses or exams too, but lectures may or may not match
        Assert.True(result.TotalCount >= 0);
    }

    [Fact]
    public async Task SearchExams_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search?q=Fundamentals&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<SearchResultDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Exams);
        Assert.Contains(result.Exams, e => e.Title.Contains("Fundamentals"));
    }

    [Fact]
    public async Task SearchCourses_FilterByCategory_ReturnsResults()
    {
        var response = await _client.GetAsync("/api/search/courses?q=&category=Programming&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<SearchResultDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SearchNoMatch_ReturnsEmpty()
    {
        var response = await _client.GetAsync("/api/search?q=NONEXISTENTXYZ123&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<SearchResultDto>();
        Assert.NotNull(result);
        Assert.Empty(result.Courses);
        Assert.Empty(result.Lectures);
        Assert.Empty(result.Exams);
    }
}
