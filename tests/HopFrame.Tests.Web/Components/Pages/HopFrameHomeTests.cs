using Bunit;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Tests.Web.Models;
using HopFrame.Web;
using HopFrame.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;

namespace HopFrame.Tests.Web.Components.Pages;

public class HopFrameHomeTests : TestContext {
    [Fact]
    public void Renders_Table_Cards_Correctly() {
        // Arrange
        var contextExplorerMock = new Mock<IContextExplorer>();
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var contextConfig = new DbContextConfig(typeof(MyDbContext));
        var tableConfigs = new List<TableConfig> {
            new TableConfig(contextConfig, typeof(MyTable), "Table1", 0) {
                DisplayName = "Table1",
                ViewPolicy = "Policy1",
                Description = "Description1"
            },
            new TableConfig(contextConfig, typeof(MyTable2), "Table2", 1) {
                DisplayName = "Table2",
                ViewPolicy = "Policy2",
                Description = "Description2"
            }
        };
        contextConfig.Tables.AddRange(tableConfigs);
        var config = new HopFrameConfig() {
            Contexts = { contextConfig }
        };

        contextExplorerMock.Setup(e => e.GetTables()).Returns(tableConfigs);
        authHandlerMock.Setup(h => h.IsAuthenticatedAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        Services.AddHopFrame(config, null, false);
        Services.AddSingleton(contextExplorerMock.Object);
        Services.AddSingleton(authHandlerMock.Object);

        // Act
        var cut = RenderComponent<HopFrameHome>();

        // Assert
        var cards = cut.FindComponents<FluentCard>();
        Assert.Equal(2, cards.Count);

        Assert.Contains(cards, card => card.Markup.Contains("Table1"));
        Assert.Contains(cards, card => card.Markup.Contains("Description1"));
        Assert.Contains(cards, card => card.Markup.Contains("Policy1"));
        Assert.Contains(cards, card => card.Markup.Contains("Open"));

        Assert.Contains(cards, card => card.Markup.Contains("Table2"));
        Assert.Contains(cards, card => card.Markup.Contains("Description2"));
        Assert.Contains(cards, card => card.Markup.Contains("Policy2"));
        Assert.Contains(cards, card => card.Markup.Contains("Open"));
    }
}