using HopFrame.Core.Config;
using HopFrame.Core.Services;
using Moq;
using Bunit;
using HopFrame.Tests.Web.Models;
using HopFrame.Web;
using HopFrame.Web.Components.Layout;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Tests.Web.Components.Layout;

public class HopFrameSideMenuTests : TestContext {
    
    [Fact]
    public void Renders_FluentAppBar_Components() {
        // Arrange
        var contextExplorerMock = new Mock<IContextExplorer>();
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var contextConfig = new DbContextConfig(typeof(MyDbContext));
        var tableConfigs = new List<TableConfig> {
            new (contextConfig, typeof(MyTable), "Table1", 0),
            new (contextConfig, typeof(MyTable2), "Table2", 1)
        };
        var config = new HopFrameConfig {
            Contexts = { contextConfig }
        };

        contextExplorerMock.Setup(e => e.GetTables()).Returns(tableConfigs);
        authHandlerMock.Setup(h => h.IsAuthenticatedAsync(null))
            .ReturnsAsync(true);

        Services.AddSingleton(contextExplorerMock.Object);
        Services.AddSingleton(authHandlerMock.Object);
        Services.AddHopFrame(config);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameSideMenu>();

        // Assert
        var items = cut.FindComponents<FluentAppBarItem>();
        Assert.Equal(tableConfigs.Count + 1, items.Count);
        Assert.Contains(items, item => item.Instance.Text.Equals("Table1"));
        Assert.Contains(items, item => item.Instance.Text.Equals("Table2"));
    }
    
}