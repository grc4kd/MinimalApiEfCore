using Api;
using Test.Fixtures;

namespace Test.AccountApi;

[Collection("CustomWebApplicationFactoryTests")]
public class SettingsTests : IAsyncDisposable
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;
    private readonly Settings _settings;

    public SettingsTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        var config = _webApplicationFactory.Services.GetRequiredService<IConfiguration>();
        var settings = config.GetRequiredSection("Settings").Get<Settings>();
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _webApplicationFactory.CleanupAsync();
    }

    [Fact]
    public void Settings_MaxDepositAmount_Check()
    {
        Assert.Equal(1000000, _settings.MaxDepositAmount);
    }

    [Fact]
    public void Settings_MaxWithdrawalAmount_Check()
    {
        Assert.Equal(1000000, _settings.MaxDepositAmount);
    }

    [Fact]
    public void Settings_MinInitialDepositAmount_Check()
    {
        Assert.Equal(100, _settings.MinInitialDepositAmount);
    }
}