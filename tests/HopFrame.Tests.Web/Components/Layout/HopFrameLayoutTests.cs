using Bunit;
using Bunit.TestDoubles;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Web;
using HopFrame.Web.Components.Layout;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;

namespace HopFrame.Tests.Web.Components.Layout;

public class HopFrameLayoutTests : TestContext {
    [Fact]
    public void Renders_HopFrameLayout_Components() {
        // Arrange
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var config = new HopFrameConfig {
            DisplayUserInfo = true,
            BasePolicy = "SomePolicy",
            LoginPageRewrite = "/login"
        };

        authHandlerMock.Setup(h => h.IsAuthenticatedAsync("SomePolicy"))
            .ReturnsAsync(true);

        Services.AddSingleton(authHandlerMock.Object);
        Services.AddHopFrame(config);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameLayout>();

        // Assert
        var header = cut.FindComponent<FluentHeader>();
        Assert.NotNull(header);

        var navigation = cut.FindComponent<HopFrameNavigation>();
        Assert.NotNull(navigation);

        var sideMenu = cut.FindComponent<HopFrameSideMenu>();
        Assert.NotNull(sideMenu);

        var footer = cut.FindComponent<FluentFooter>();
        Assert.NotNull(footer);
    }

    [Fact]
    public void Redirects_To_Login_When_Not_Authorized() {
        // Arrange
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var navMock = new FakeNavigationManager(this);
        var config = new HopFrameConfig {
            DisplayUserInfo = true,
            BasePolicy = "SomePolicy",
            LoginPageRewrite = "/login"
        };

        authHandlerMock.Setup(h => h.IsAuthenticatedAsync("SomePolicy"))
            .ReturnsAsync(false);

        Services.AddSingleton(navMock);
        Services.AddHopFrame(config);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameLayout>();

        // Assert
        // TODO: check if uri matches
    }
}