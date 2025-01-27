using Bunit;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Tests.Web.Models;
using HopFrame.Web;
using HopFrame.Web.Components.Dialogs;
using HopFrame.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;

namespace HopFrame.Tests.Web.Components.Dialogs;

public class HopFrameEditorTests : TestContext {
    [Fact]
    public void Renders_Properties_Correctly() {
        // Arrange
        var contextExplorerMock = new Mock<IContextExplorer>();
        var authHandlerMock = new Mock<IHopFrameAuthHandler>();
        var dialogServiceMock = new Mock<IDialogService>();
        var toastServiceMock = new Mock<IToastService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var contextConfig = new DbContextConfig(typeof(MyDbContext));
        var tableConfig = new TableConfig(contextConfig, typeof(MyTable), "Table1", 0) {
            DisplayName = "Table1",
            ViewPolicy = "Policy1"
        };
        contextConfig.Tables.Add(tableConfig);
        var config = new HopFrameConfig() {
            Contexts = { contextConfig }
        };

        contextExplorerMock.Setup(e => e.GetTable("Table1")).Returns(tableConfig);
        contextExplorerMock.Setup(e => e.GetTableManager("Table1")).Returns(Mock.Of<ITableManager>());
        authHandlerMock.Setup(h => h.IsAuthenticatedAsync(It.IsAny<string>())).ReturnsAsync(true);

        Services.AddHopFrame(config, null, false);
        Services.AddSingleton(contextExplorerMock.Object);
        Services.AddSingleton(authHandlerMock.Object);
        Services.AddSingleton(dialogServiceMock.Object);
        Services.AddSingleton(toastServiceMock.Object);
        Services.AddSingleton(serviceProviderMock.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;

        var dialogData = new EditorDialogData(tableConfig, new MyTable());
        var dialog = new FluentDialog() {
            Instance = new DialogInstance(typeof(HopFrameEditor), new DialogParameters(), dialogData)
        };

        // Act
        var cut = RenderComponent<HopFrameEditor>(parameters => parameters
            .Add(p => p.Content, dialogData)
            .Add(p => p.Dialog, dialog));

        // Assert
        var dialogBody = cut.FindComponent<FluentDialogBody>();
        Assert.NotNull(dialogBody);

        var textFields = cut.FindComponents<FluentNumberField<double>>();
        Assert.Single(textFields);
    }
}