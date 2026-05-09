using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Announcements;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class AnnouncementsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AnnouncementsControllerTests()
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
    public async Task GetAnnouncements_Returns200()
    {
        var response = await _client.GetAsync("/api/announcements");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<AnnouncementDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, a => a.Title == "Platform Update");
    }

    [Fact]
    public async Task GetAnnouncementById_Returns200()
    {
        var announcementId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var response = await _client.GetAsync($"/api/announcements/{announcementId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AnnouncementDto>();
        Assert.NotNull(result);
        Assert.Equal("Platform Update", result.Title);
        Assert.True(result.IsPublished);
    }

    [Fact]
    public async Task CreateAnnouncement_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateAnnouncementRequestDto
        {
            Title = "New Lecture Available",
            Content = "A new lecture has been added to the course.",
            Target = AnnouncementTarget.All,
            Priority = 2
        };

        var response = await _client.PostAsJsonAsync("/api/announcements", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AnnouncementDto>();
        Assert.NotNull(result);
        Assert.Equal("New Lecture Available", result.Title);
        Assert.False(result.IsPublished);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateAnnouncement_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateAnnouncementRequestDto
        {
            Title = "Test",
            Content = "Test content"
        };

        var response = await _client.PostAsJsonAsync("/api/announcements", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task PublishAnnouncement_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var createRequest = new CreateAnnouncementRequestDto
        {
            Title = "Draft Announcement",
            Content = "This is a draft.",
            Target = AnnouncementTarget.All
        };

        var createResponse = await _client.PostAsJsonAsync("/api/announcements", createRequest);
        var created = await createResponse.Content.ReadApiResponseAsync<AnnouncementDto>();
        Assert.NotNull(created);

        var publishResponse = await _client.PostAsync($"/api/announcements/{created.Id}/publish", null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var result = await publishResponse.Content.ReadApiResponseAsync<AnnouncementDto>();
        Assert.NotNull(result);
        Assert.True(result.IsPublished);
        Assert.NotNull(result.PublishedAt);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DeleteAnnouncement_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var createRequest = new CreateAnnouncementRequestDto
        {
            Title = "To Delete",
            Content = "Will be deleted"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/announcements", createRequest);
        var created = await createResponse.Content.ReadApiResponseAsync<AnnouncementDto>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/announcements/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsync("/api/announcements", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
