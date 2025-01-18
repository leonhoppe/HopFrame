using HopFrame.Core.Config;
using HopFrame.Core.Tests.Models;

namespace HopFrame.Core.Tests.Config;

public class HopFrameConfiguratorTests {
    [Fact]
    public void AddDbContext_AddsDbContextToInnerConfig() {
        // Arrange
        var config = new HopFrameConfig();
        var configurator = new HopFrameConfigurator(config);

        // Act
        var dbContextConfigurator = configurator.AddDbContext<MockDbContext>();

        // Assert
        Assert.Single(config.Contexts);
        Assert.IsType<DbContextConfig>(config.Contexts[0]);
        Assert.IsType<DbContextConfigurator<MockDbContext>>(dbContextConfigurator);
    }

    [Fact]
    public void DisplayUserInfo_SetsDisplayUserInfoProperty() {
        // Arrange
        var config = new HopFrameConfig();
        var configurator = new HopFrameConfigurator(config);

        // Act
        configurator.DisplayUserInfo(false);

        // Assert
        Assert.False(config.DisplayUserInfo);
    }

    [Fact]
    public void SetBasePolicy_SetsBasePolicyProperty() {
        // Arrange
        var config = new HopFrameConfig();
        var configurator = new HopFrameConfigurator(config);
        var basePolicy = "Admin";

        // Act
        configurator.SetBasePolicy(basePolicy);

        // Assert
        Assert.Equal(basePolicy, config.BasePolicy);
    }

    [Fact]
    public void SetLoginPage_SetsLoginPageRewriteProperty() {
        // Arrange
        var config = new HopFrameConfig();
        var configurator = new HopFrameConfigurator(config);
        var loginPageUrl = "/login";

        // Act
        configurator.SetLoginPage(loginPageUrl);

        // Assert
        Assert.Equal(loginPageUrl, config.LoginPageRewrite);
    }
}
