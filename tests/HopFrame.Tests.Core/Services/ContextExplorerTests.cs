using HopFrame.Core.Config;
using HopFrame.Core.Services.Implementations;
using HopFrame.Tests.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace HopFrame.Tests.Core.Services;

public class ContextExplorerTests {
    [Fact]
    public void GetTables_ReturnsNonIgnoredTables() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig1 = contextConfig.Tables[0];
        var tableConfig2 = contextConfig.Tables[1];
        config.Contexts.Add(contextConfig);
        tableConfig2.Ignored = true;

        var provider = new Mock<IServiceProvider>();
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var tables = contextExplorer.GetTables().ToList();

        // Assert
        Assert.Single(tables);
        Assert.Contains(tableConfig1, tables);
        Assert.DoesNotContain(tableConfig2, tables);
    }

    [Fact]
    public void GetTable_ByDisplayName_ReturnsCorrectTable() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        config.Contexts.Add(contextConfig);
        tableConfig.DisplayName = "TestTable";

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var result = contextExplorer.GetTable("TestTable");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tableConfig, result);
    }
    
    [Fact]
    public void GetTable_ByDisplayName_ReturnsNullIfTableNotFound() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        config.Contexts.Add(contextConfig);
        tableConfig.DisplayName = "TestTable";

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var result = contextExplorer.GetTable("InvalidTable");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTable_ByType_ReturnsCorrectTable() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        config.Contexts.Add(contextConfig);

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var result = contextExplorer.GetTable(typeof(MockModel));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tableConfig, result);
    }
    
    [Fact]
    public void GetTable_ByType_ReturnsNullIfTableNotFound() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        config.Contexts.Add(contextConfig);

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var result = contextExplorer.GetTable(typeof(ContextExplorerTests));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTableManager_ReturnsCorrectTableManager() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = new TableConfig(contextConfig, typeof(MockModel), "Models", 0);
        contextConfig.Tables.Add(tableConfig);
        config.Contexts.Add(contextConfig);

        var dbContext = new MockDbContext();
        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(dbContext);
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var tableManager = contextExplorer.GetTableManager("Models");

        // Assert
        Assert.NotNull(tableManager);
        Assert.IsType<TableManager<MockModel>>(tableManager);
    }

    [Fact]
    public void GetTableManager_ReturnsNullIfDbContextNotFound() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = new TableConfig(contextConfig, typeof(MockModel), "MockModels", 0);
        contextConfig.Tables.Add(tableConfig);
        config.Contexts.Add(contextConfig);

        var provider = new Mock<IServiceProvider>();
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var tableManager = contextExplorer.GetTableManager("Models");

        // Assert
        Assert.Null(tableManager);
    }
    
    [Fact]
    public void GetTableManager_ReturnsNullIfTableNotFound() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = new TableConfig(contextConfig, typeof(MockModel), "Models", 0);
        contextConfig.Tables.Add(tableConfig);
        config.Contexts.Add(contextConfig);

        var dbContext = new MockDbContext();
        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(dbContext);
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        var tableManager = contextExplorer.GetTableManager("InvalidTable");

        // Assert
        Assert.Null(tableManager);
    }

    [Fact]
    public void SeedTableData_SetsTableSeededFlag() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        config.Contexts.Add(contextConfig);

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        contextExplorer.GetTable("Models");

        // Assert
        Assert.True(tableConfig.Seeded);
    }
    
    [Fact]
    public void SeedTableData_SetsTablePropertiesCorrectly() {
        // Arrange
        var config = new HopFrameConfig();
        var contextConfig = new DbContextConfig(typeof(MockDbContext));
        var tableConfig = contextConfig.Tables[0];
        var tableConfig2 = contextConfig.Tables[1];
        config.Contexts.Add(contextConfig);

        var provider = new Mock<IServiceProvider>();
        provider.Setup(p => p.GetService(typeof(MockDbContext))).Returns(new MockDbContext());
        var contextExplorer = new ContextExplorer(config, provider.Object, new Logger<ContextExplorer>(new LoggerFactory()));

        // Act
        contextExplorer.GetTable("Models");
        contextExplorer.GetTable("Models2");

        // Assert
        var relationProp = tableConfig.Properties.SingleOrDefault(prop => prop.Info.Name == nameof(MockModel.Model2));
        var keyProp = tableConfig2.Properties.SingleOrDefault(prop => prop.Info.Name == nameof(MockModel2.Id));
        Assert.NotNull(relationProp);
        Assert.NotNull(keyProp);
        Assert.True(relationProp.IsRelation);
        Assert.True(relationProp.IsEnumerable);
        Assert.True(keyProp.IsRequired);
        Assert.False(keyProp.IsRelation);
        Assert.False(keyProp.IsEnumerable);
    }
}