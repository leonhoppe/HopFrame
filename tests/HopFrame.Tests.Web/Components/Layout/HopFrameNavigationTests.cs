using Bunit;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Web;
using HopFrame.Web.Components.Layout;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;

namespace HopFrame.Tests.Web.Components.Layout;

public class HopFrameNavigationTests : TestContext {
    [Fact]
    public void Renders_HopFrameNavigation_Components() {
        // Arrange
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var config = new HopFrameConfig {
            DisplayUserInfo = true
        };

        authHandlerMock.Setup(h => h.GetCurrentUserDisplayNameAsync())
            .ReturnsAsync("John Doe");

        Services.AddSingleton(authHandlerMock.Object);
        Services.AddHopFrame(config);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameNavigation>();

        // Assert
        var header = cut.FindComponent<FluentHeader>();
        Assert.NotNull(header);

        var persona = cut.FindComponent<FluentPersona>();
        Assert.NotNull(persona);
        Assert.Equal("John Doe", persona.Instance.Name);
        Assert.Equal("JD", persona.Instance.Initials);
    }

    [Fact]
    public void Renders_HopFrameNavigation_WithoutPersona() {
        // Arrange
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var config = new HopFrameConfig {
            DisplayUserInfo = false
        };

        authHandlerMock.Setup(h => h.GetCurrentUserDisplayNameAsync())
            .ReturnsAsync("John Doe");

        Services.AddSingleton(authHandlerMock.Object);
        Services.AddHopFrame(config);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameNavigation>();

        // Assert
        var header = cut.FindComponent<FluentHeader>();
        Assert.NotNull(header);
        Assert.False(cut.HasComponent<FluentPersona>());
        
        authHandlerMock.Verify(h => h.GetCurrentUserDisplayNameAsync(), Times.Never);
    }
}