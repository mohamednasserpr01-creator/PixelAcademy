using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class ActivationCodesControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ActivationCodesControllerTests()
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
    public async Task Generate_WalletCreditCode_AsAdmin_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.WalletCredit,
            Value = 50.00m,
            MaxRedemptions = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/generate", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(result);
        Assert.Equal(CodeType.WalletCredit, result.Type);
        Assert.Equal(50.00m, result.Value);
        Assert.Equal(5, result.MaxRedemptions);
        Assert.Equal(0, result.CurrentRedemptions);
        Assert.True(result.IsActive);
        Assert.False(result.IsFullyRedeemed);
        Assert.NotEmpty(result.Code);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Generate_CourseEnrollmentCode_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.CourseEnrollment,
            CourseId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/generate", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(result);
        Assert.Equal(CodeType.CourseEnrollment, result.Type);
        Assert.Equal(Guid.Parse("55555555-5555-5555-5555-555555555555"), result.CourseId);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Generate_LectureAccessCode_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.LectureAccess,
            LectureId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/generate", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(result);
        Assert.Equal(CodeType.LectureAccess, result.Type);
        Assert.Equal(Guid.Parse("77777777-7777-7777-7777-777777777777"), result.LectureId);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_WalletCreditCode_UpdatesBalance()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RedeemActivationCodeRequestDto
        {
            Code = "PA-WALLET-TEST01"
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<RedeemResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(CodeType.WalletCredit, result.Type);
        Assert.Equal(100.00m, result.NewWalletBalance);
        Assert.Contains("credits", result.Message);
        Assert.Equal(0, result.RemainingRedemptions);

        // Verify balance via /api/auth/me
        var meResponse = await _client.GetAsync("/api/auth/me");
        var meResult = await meResponse.Content.ReadApiResponseAsync<UserDto>();
        Assert.NotNull(meResult);
        Assert.Equal(100.00m, meResult.WalletBalance);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_CourseEnrollmentCode_EnrollsStudent()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RedeemActivationCodeRequestDto
        {
            Code = "PA-ENROLL-TEST01"
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<RedeemResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(CodeType.CourseEnrollment, result.Type);
        Assert.Equal(Guid.Parse("44444444-4444-4444-4444-444444444444"), result.CourseId);
        Assert.Equal(0, result.RemainingRedemptions);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_LectureAccessCode_GrantsAccess()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RedeemActivationCodeRequestDto
        {
            Code = "PA-LECTURE-TEST01"
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<RedeemResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(CodeType.LectureAccess, result.Type);
        Assert.Equal(Guid.Parse("77777777-7777-7777-7777-777777777777"), result.LectureId);
        Assert.Equal(0, result.RemainingRedemptions);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_FullyRedeemedCode_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // First redeem succeeds
        var firstRequest = new RedeemActivationCodeRequestDto { Code = "PA-WALLET-TEST01" };
        var firstResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", firstRequest);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Second redeem with same code fails (MaxRedemptions = 1)
        var secondResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", firstRequest);
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_ExpiredCode_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RedeemActivationCodeRequestDto
        {
            Code = "PA-EXPIRED-TEST01"
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_DisabledCode_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RedeemActivationCodeRequestDto
        {
            Code = "PA-DISABLED-TEST01"
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Redeem_DuplicatePersonalRedeem_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Redeem enrollment code
        var request = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var firstResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Try to redeem same enrollment code again (same student) - should fail as duplicate personal redeem
        // Note: PA-ENROLL-TEST01 has MaxRedemptions=1, so this would also fail for that reason,
        // but the handler checks personal redemption first for non-wallet types.
        // We use a multi-redeem enrollment code to properly test the duplicate personal check,
        // but we don't have one seeded. Let's generate one first.
        _client.DefaultRequestHeaders.Authorization = null;

        var instructorAuth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", instructorAuth.AccessToken);

        var genRequest = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.CourseEnrollment,
            CourseId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            MaxRedemptions = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var genResponse = await _client.PostAsJsonAsync("/api/activationcodes/generate", genRequest);
        var genResult = await genResponse.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(genResult);

        _client.DefaultRequestHeaders.Authorization = null;

        // Student redeems the multi-use enrollment code
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        var redeemRequest = new RedeemActivationCodeRequestDto { Code = genResult.Code };
        var redeemResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", redeemRequest);
        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);

        // Student tries to redeem same code again - should fail as duplicate personal
        var duplicateResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", redeemRequest);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DisableCode_AsGenerator_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Generate a new code
        var genRequest = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.WalletCredit,
            Value = 10.00m,
            MaxRedemptions = 1
        };
        var genResponse = await _client.PostAsJsonAsync("/api/activationcodes/generate", genRequest);
        var genResult = await genResponse.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(genResult);

        // Disable it
        var disableResponse = await _client.PostAsync($"/api/activationcodes/{genResult.Id}/disable", null);
        Assert.Equal(HttpStatusCode.NoContent, disableResponse.StatusCode);

        // Verify it's disabled
        var getResponse = await _client.GetAsync($"/api/activationcodes/{genResult.Id}");
        var getResult = await getResponse.Content.ReadApiResponseAsync<ActivationCodeDto>();
        Assert.NotNull(getResult);
        Assert.False(getResult.IsActive);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GenerateCode_AsStudent_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new GenerateActivationCodeRequestDto
        {
            Type = CodeType.WalletCredit,
            Value = 10.00m
        };

        var response = await _client.PostAsJsonAsync("/api/activationcodes/generate", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task MultiRedeemCode_AllowsUpToMax()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // PA-MULTI-TEST01 has MaxRedemptions=3, Value=10
        var request = new RedeemActivationCodeRequestDto { Code = "PA-MULTI-TEST01" };

        // Redeem 3 times
        for (int i = 1; i <= 3; i++)
        {
            var response = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadApiResponseAsync<RedeemResultDto>();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(10.00m * i, result.NewWalletBalance);
            Assert.Equal(3 - i, result.RemainingRedemptions);
        }

        // 4th redeem should fail (max reached)
        var failResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);
        Assert.Equal(HttpStatusCode.BadRequest, failResponse.StatusCode);

        // Verify final balance
        var meResponse = await _client.GetAsync("/api/auth/me");
        var meResult = await meResponse.Content.ReadApiResponseAsync<UserDto>();
        Assert.NotNull(meResult);
        Assert.Equal(30.00m, meResult.WalletBalance);

        _client.DefaultRequestHeaders.Authorization = null;
    }
}
