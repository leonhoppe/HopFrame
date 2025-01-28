using Bunit;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Tests.Web.Helpers;
using HopFrame.Tests.Web.Models;
using HopFrame.Web;
using HopFrame.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace HopFrame.Tests.Web.Components.Pages;

public class HopFrameTablePageTests : TestContext {
    [Fact]
    public void Renders_Table_Correctly() {
        // Arrange
        var contextExplorerMock = new Mock<IContextExplorer>();
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var dialogServiceMock = new Mock<IDialogService>();
        var contextConfig = new DbContextConfig(typeof(MyDbContext));
        var managerMock = new Mock<ITableManager>();
        var tableConfig = new TableConfig(contextConfig, typeof(MyTable), "Table1", 0) {
            DisplayName = "Table1",
            ViewPolicy = "Policy1"
        };
        contextConfig.Tables.Add(tableConfig);
        var config = new HopFrameConfig() {
            Contexts = { contextConfig }
        };

        contextExplorerMock.Setup(e => e.GetTable("Table1")).Returns(tableConfig);
        contextExplorerMock.Setup(e => e.GetTableManager("Table1")).Returns(managerMock.Object);
        authHandlerMock.Setup(h => h.IsAuthenticatedAsync(It.IsAny<string>())).ReturnsAsync(true);
        managerMock.Setup(m => m.LoadPage(It.IsAny<int>(), It.IsAny<int>())).Returns(Enumerable.Empty<object>().AsAsyncQueryable());

        Services.AddHopFrame(config, null, false);
        Services.AddSingleton(contextExplorerMock.Object);
        Services.AddSingleton(authHandlerMock.Object);
        Services.AddSingleton(dialogServiceMock.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameTablePage>(parameters => parameters
            .Add(p => p.TableDisplayName, "Table1"));

        // Assert
        var toolbar = cut.Find("fluent-toolbar");
        Assert.NotNull(toolbar);

        var searchBox = cut.Find("fluent-search");
        Assert.NotNull(searchBox);

        var dataGrid = cut.FindComponent<FluentDataGrid<object>>();
        Assert.NotNull(dataGrid);
    }

    [Fact]
    public void Displays_Properties_Correctly() {
        // Arrange
        var contextExplorerMock = new Mock<IContextExplorer>();
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var dialogServiceMock = new Mock<IDialogService>();
        var contextConfig = new DbContextConfig(typeof(MyDbContext));
        var tableConfig = new TableConfig(contextConfig, typeof(MyTable), "Table1", 0) {
            DisplayName = "Table1",
            ViewPolicy = "Policy1"
        };

        var tableManagerMock = new Mock<ITableManager>();
        var items = new List<object> { new MyTable(), new MyTable() };
        tableManagerMock.Setup(m => m.LoadPage(It.IsAny<int>(), It.IsAny<int>())).Returns(items.AsAsyncQueryable());
        tableManagerMock.Setup(t => t.DisplayProperty(It.IsAny<object>(), It.IsAny<PropertyConfig>(), null, null))
            .ReturnsAsync(string.Empty);

        contextExplorerMock.Setup(e => e.GetTable("Table1")).Returns(tableConfig);
        contextExplorerMock.Setup(e => e.GetTableManager("Table1")).Returns(tableManagerMock.Object);
        authHandlerMock.Setup(h => h.IsAuthenticatedAsync(It.IsAny<string>())).ReturnsAsync(true);

        Services.AddHopFrame(new HopFrameConfig(), null, false);
        Services.AddSingleton(contextExplorerMock.Object);
        Services.AddSingleton(authHandlerMock.Object);
        Services.AddSingleton(dialogServiceMock.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<HopFrameTablePage>(parameters => parameters
            .Add(p => p.TableDisplayName, "Table1"));

        // Assert
        var dataGridItems = cut.FindComponents<PropertyColumn<object, string>>();
        Assert.Single(dataGridItems);
        Assert.Equal(nameof(MyTable.Id), dataGridItems[0].Instance.Title);
    }
}