using HopFrame.Core.Config;
using HopFrame.Tests.Core.Models;
using Moq;

namespace HopFrame.Tests.Core.Config;

public class DbContextConfiguratorTests {
    [Fact]
    public void Table_WithConfigurator_InvokesConfigurator() {
        // Arrange
        var dbContextConfig = new DbContextConfig(typeof(MockDbContext), null!);
        var configurator = new DbContextConfigurator<MockDbContext>(dbContextConfig);
        var mockConfigurator = new Mock<Action<TableConfigurator<MockModel>>>();

        // Act
        configurator.Table<MockModel>(mockConfigurator.Object);

        // Assert
        mockConfigurator.Verify(c => c.Invoke(It.IsAny<TableConfigurator<MockModel>>()), Times.Once);
    }

    [Fact]
    public void Table_ReturnsCorrectTableConfigurator() {
        // Arrange
        var dbContextConfig = new DbContextConfig(typeof(MockDbContext), null!);
        var configurator = new DbContextConfigurator<MockDbContext>(dbContextConfig);

        // Act
        var tableConfigurator = configurator.Table<MockModel>();

        // Assert
        Assert.IsType<TableConfigurator<MockModel>>(tableConfigurator);
    }
}