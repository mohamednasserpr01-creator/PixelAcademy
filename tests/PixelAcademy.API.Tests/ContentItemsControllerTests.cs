using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class ContentItemsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ContentItemsControllerTests()
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
    public async Task CreateContentItem_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var request = new CreateContentItemRequestDto
        {
            Title = "New Video Content",
            Description = "A new video content item.",
            OrderIndex = 10,
            Type = ContentItemType.Video,
            IsRequired = true,
            DurationSeconds = 600,
            ExternalUrl = "https://example.com/video"
        };

        var response = await _client.PostAsJsonAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ContentItemDto>();
        Assert.NotNull(result);
        Assert.Equal("New Video Content", result.Title);
        Assert.Equal(10, result.OrderIndex);
        Assert.Equal(ContentItemType.Video, result.Type);
        Assert.True(result.IsRequired);
        Assert.Equal(600, result.DurationSeconds);
        Assert.Equal("https://example.com/video", result.ExternalUrl);
        Assert.Equal(lectureId, result.LectureId);
        Assert.NotEqual(Guid.Empty, result.Id);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateContentItem_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var request = new CreateContentItemRequestDto
        {
            Title = "Student Content Attempt",
            Description = "Should fail.",
            OrderIndex = 99,
            Type = ContentItemType.PDF,
            IsRequired = false
        };

        var response = await _client.PostAsJsonAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetContentItemsByLecture_Returns200_OrderedByIndex()
    {
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var response = await _client.GetAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<ContentItemDto>>();
        Assert.NotNull(result);
        Assert.True(result.Count >= 4);

        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].OrderIndex <= result[i + 1].OrderIndex,
                $"Content items should be ordered by OrderIndex. Item[{i}].OrderIndex={result[i].OrderIndex} > Item[{i+1}].OrderIndex={result[i+1].OrderIndex}");
        }
    }

    [Fact]
    public async Task GetContentItemById_Returns200()
    {
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var contentItemId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        var response = await _client.GetAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{contentItemId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ContentItemDto>();
        Assert.NotNull(result);
        Assert.Equal("Introduction Video", result.Title);
        Assert.Equal(ContentItemType.Video, result.Type);
        Assert.Equal(1, result.OrderIndex);
        Assert.True(result.IsRequired);
        Assert.Equal(300, result.DurationSeconds);
    }

    [Fact]
    public async Task UpdateContentItem_AsInstructor_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var contentItemId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var request = new UpdateContentItemRequestDto
        {
            Title = "Updated Introduction Video",
            Description = "Updated description.",
            OrderIndex = 5,
            Type = ContentItemType.Video,
            IsRequired = false,
            DurationSeconds = 450,
            ExternalUrl = "https://example.com/updated"
        };

        var response = await _client.PutAsJsonAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{contentItemId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ContentItemDto>();
        Assert.NotNull(result);
        Assert.Equal("Updated Introduction Video", result.Title);
        Assert.Equal(5, result.OrderIndex);
        Assert.False(result.IsRequired);
        Assert.Equal(450, result.DurationSeconds);
        Assert.Equal("https://example.com/updated", result.ExternalUrl);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DeleteContentItem_AsInstructor_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var contentItemId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        var response = await _client.DeleteAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{contentItemId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetDeletedContentItem_Returns404()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var contentItemId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        var deleteResponse = await _client.DeleteAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{contentItemId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;

        var getResponse = await _client.GetAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{contentItemId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetContentItemById_NotFound_Returns404()
    {
        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var nonExistentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var response = await _client.GetAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateContentItem_WithAllTypes_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var types = new[] { ContentItemType.PDF, ContentItemType.Exam, ContentItemType.Homework };

        for (int i = 0; i < types.Length; i++)
        {
            var request = new CreateContentItemRequestDto
            {
                Title = $"{types[i]} Item {i}",
                Description = $"A {types[i]} content item.",
                OrderIndex = 100 + i,
                Type = types[i],
                IsRequired = true
            };

            var response = await _client.PostAsJsonAsync($"/api/courses/44444444-4444-4444-4444-444444444444/lectures/{lectureId}/contentitems", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadApiResponseAsync<ContentItemDto>();
            Assert.NotNull(result);
            Assert.Equal(types[i], result.Type);
        }

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
