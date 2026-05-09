using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Application.DTOs.Common;
using Xunit;

namespace PixelAcademy.API.Tests;

public class WalletControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public WalletControllerTests()
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
    public async Task GetBalance_ReturnsZero_ForNewStudent()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/wallet/balance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<WalletBalanceDto>();
        Assert.NotNull(result);
        Assert.Equal(0.00m, result.Balance);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetBalance_ReturnsUpdatedBalance_AfterRedeem()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Redeem wallet credit code
        var redeemRequest = new RedeemActivationCodeRequestDto { Code = "PA-WALLET-TEST01" };
        var redeemResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", redeemRequest);
        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);

        // Check balance
        var balanceResponse = await _client.GetAsync("/api/wallet/balance");
        Assert.Equal(HttpStatusCode.OK, balanceResponse.StatusCode);
        var balanceResult = await balanceResponse.Content.ReadApiResponseAsync<WalletBalanceDto>();
        Assert.NotNull(balanceResult);
        Assert.Equal(100.00m, balanceResult.Balance);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetTransactions_ReturnsEmpty_ForNewStudent()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/wallet/transactions?pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<WalletTransactionDto>>();
        Assert.NotNull(result);
        Assert.Empty(result);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetTransactions_ReturnsRechargeTransaction_AfterRedeem()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Redeem wallet credit code
        var redeemRequest = new RedeemActivationCodeRequestDto { Code = "PA-WALLET-TEST01" };
        var redeemResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", redeemRequest);
        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);

        // Get wallet transactions
        var txResponse = await _client.GetAsync("/api/wallet/transactions?pageSize=100");
        Assert.Equal(HttpStatusCode.OK, txResponse.StatusCode);

        var txResult = await txResponse.Content.ReadApiResponseAsync<List<WalletTransactionDto>>();
        Assert.NotNull(txResult);
        Assert.Single(txResult);

        var tx = txResult[0];
        Assert.Equal(WalletTransactionType.Recharge, tx.Type);
        Assert.Equal(100.00m, tx.Amount);
        Assert.Equal(0.00m, tx.BalanceBefore);
        Assert.Equal(100.00m, tx.BalanceAfter);
        Assert.Contains("PA-WALLET-TEST01", tx.Description);
        Assert.Equal("PA-WALLET-TEST01", tx.Code);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetTransactions_MultipleRedeems_ShowCorrectBalances()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Redeem multi-use code 3 times
        var request = new RedeemActivationCodeRequestDto { Code = "PA-MULTI-TEST01" };
        for (int i = 0; i < 3; i++)
        {
            var redeemResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", request);
            Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);
        }

        // Get wallet transactions
        var txResponse = await _client.GetAsync("/api/wallet/transactions?pageSize=100");
        Assert.Equal(HttpStatusCode.OK, txResponse.StatusCode);

        var txResult = await txResponse.Content.ReadApiResponseAsync<List<WalletTransactionDto>>();
        Assert.NotNull(txResult);
        Assert.Equal(3, txResult.Count);

        // Verify progressive balances
        Assert.Equal(0.00m, txResult[2].BalanceBefore);
        Assert.Equal(10.00m, txResult[2].BalanceAfter);
        Assert.Equal(10.00m, txResult[1].BalanceBefore);
        Assert.Equal(20.00m, txResult[1].BalanceAfter);
        Assert.Equal(20.00m, txResult[0].BalanceBefore);
        Assert.Equal(30.00m, txResult[0].BalanceAfter);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetBalance_Unauthorized_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/wallet/balance");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactions_Unauthorized_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/wallet/transactions");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
