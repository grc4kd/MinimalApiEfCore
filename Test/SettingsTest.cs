using System.ComponentModel.DataAnnotations;
using Api;
using Api.Request;

namespace Test;

public class SettingsTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Settings _settings;

    public SettingsTest(CustomWebApplicationFactory<Program> factory) {
        _factory = factory;
        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var settings = config.GetRequiredSection("Settings").Get<Settings>();
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
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