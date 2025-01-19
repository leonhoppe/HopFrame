using System.Linq.Expressions;
using HopFrame.Core.Config;
using HopFrame.Core.Tests.Models;

namespace HopFrame.Core.Tests.Config;

public class TableConfiguratorTests {
    [Fact]
    public void Ignore_SetsIgnoredProperty() {
        // Arrange
        var tableConfig =
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);

        // Act
        configurator.Ignore(true);

        // Assert
        Assert.True(tableConfig.Ignored);
    }

    [Fact]
    public void Property_ReturnsCorrectPropertyConfigurator() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        Expression<Func<MockModel, int>> propertyExpression = model => model.Id;

        // Act
        var propertyConfigurator = configurator.Property(propertyExpression);

        // Assert
        Assert.IsType<PropertyConfigurator<int>>(propertyConfigurator);
    }
    
    public void Property_WithConfigurator_ReturnsCorrectPropertyConfigurator() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        Expression<Func<MockModel, int>> propertyExpression = model => model.Id;

        // Act
        object propertyConfigurator = null!;
        configurator.Property(propertyExpression, c => {
            propertyConfigurator = c;
        });

        // Assert
        Assert.IsType<PropertyConfigurator<int>>(propertyConfigurator);
    }

    [Fact]
    public void AddVirtualProperty_AddsVirtualPropertyToConfig() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        Func<MockModel, IServiceProvider, string> template = (model, _) => model.Name!;

        // Act
        var propertyConfigurator = configurator.AddVirtualProperty("VirtualName", template);

        // Assert
        var virtualProperty = tableConfig.Properties.SingleOrDefault(p => p.Name == "VirtualName");
        Assert.NotNull(virtualProperty);
        Assert.NotNull(propertyConfigurator);
        Assert.True(virtualProperty.IsListingProperty);
        Assert.Equal("VirtualName", virtualProperty.Name);
    }
    
    [Fact]
    public void AddVirtualProperty_WithConfigurator_AddsVirtualPropertyToConfig() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        Func<MockModel, IServiceProvider, string> template = (model, _) => model.Name!;

        // Act
        object propertyConfigurator = null!;
        configurator.AddVirtualProperty("VirtualName", template, c => {
            propertyConfigurator = c;
        });

        // Assert
        var virtualProperty = tableConfig.Properties.SingleOrDefault(p => p.Name == "VirtualName");
        Assert.NotNull(virtualProperty);
        Assert.NotNull(propertyConfigurator);
        Assert.True(virtualProperty.IsListingProperty);
        Assert.Equal("VirtualName", virtualProperty.Name);
    }

    [Fact]
    public void SetDisplayName_SetsDisplayNameProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var displayName = "Mock Model Display Name";

        // Act
        configurator.SetDisplayName(displayName);

        // Assert
        Assert.Equal(displayName, tableConfig.DisplayName);
    }

    [Fact]
    public void SetDescription_SetsDescriptionProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var description = "Mock Model Description";

        // Act
        configurator.SetDescription(description);

        // Assert
        Assert.Equal(description, tableConfig.Description);
    }

    [Fact]
    public void SetOrderIndex_SetsOrderIndexProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var orderIndex = 1;

        // Act
        configurator.SetOrderIndex(orderIndex);

        // Assert
        Assert.Equal(orderIndex, tableConfig.Order);
    }

    [Fact]
    public void SetViewPolicy_SetsViewPolicyProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var policy = "ViewPolicy";

        // Act
        configurator.SetViewPolicy(policy);

        // Assert
        Assert.Equal(policy, tableConfig.ViewPolicy);
    }

    [Fact]
    public void SetUpdatePolicy_SetsUpdatePolicyProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var policy = "UpdatePolicy";

        // Act
        configurator.SetUpdatePolicy(policy);

        // Assert
        Assert.Equal(policy, tableConfig.UpdatePolicy);
    }

    [Fact]
    public void SetCreatePolicy_SetsCreatePolicyProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var policy = "CreatePolicy";

        // Act
        configurator.SetCreatePolicy(policy);

        // Assert
        Assert.Equal(policy, tableConfig.CreatePolicy);
    }

    [Fact]
    public void SetDeletePolicy_SetsDeletePolicyProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        var configurator = new TableConfigurator<MockModel>(tableConfig);
        var policy = "DeletePolicy";

        // Act
        configurator.SetDeletePolicy(policy);

        // Assert
        Assert.Equal(policy, tableConfig.DeletePolicy);
    }

    [Fact]
    public void Constructor_WithKeyProperty_DisablesEdit() {
        // Act
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel2), "Models2", 0);
        var prop = tableConfig.Properties.SingleOrDefault(prop => prop.Info.Name == nameof(MockModel2.Id));

        // Assert
        Assert.NotNull(prop);
        Assert.False(prop.Editable);
    }

    [Fact]
    public void Constructor_WithGeneratedProperty_DisablesEditAndCreate() {
        // Act
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel2), "Models2", 0);
        var prop = tableConfig.Properties.SingleOrDefault(prop => prop.Info.Name == nameof(MockModel2.Number));

        // Assert
        Assert.NotNull(prop);
        Assert.False(prop.Editable);
        Assert.False(prop.Creatable);
    }
}