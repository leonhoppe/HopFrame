using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementations;
using HopFrame.Tests.Core.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HopFrame.Tests.Core.Services;

public class DisplayPropertyTests {
    private readonly Mock<IServiceProvider> _providerMock;
    private readonly Mock<IContextExplorer> _explorerMock;
    private readonly TableConfig _config;
    private readonly TableManager<object> _tableManager;

    public DisplayPropertyTests() {
        var contextMock = new Mock<DbContext>();
        _providerMock = new Mock<IServiceProvider>();
        _explorerMock = new Mock<IContextExplorer>();
        _config = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "Models", 0);
        _tableManager =
            new TableManager<object>(contextMock.Object, _config, _explorerMock.Object, _providerMock.Object);
    }

    [Fact]
    public void DisplayProperty_ReturnsEmptyString_WhenItemIsNull() {
        // Arrange
        var prop = new PropertyConfig(typeof(string).GetProperty("Length")!, _config, 0);

        // Act
        var result = _tableManager.DisplayProperty(null, prop);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void DisplayProperty_UsesFormatter_WhenListingProperty() {
        // Arrange
        var item = "test";
        var prop = new PropertyConfig(typeof(string).GetProperty("Length")!, _config, 0) {
            IsListingProperty = true,
            Formatter = (obj, provider) => ((string)obj).ToUpper()
        };

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("TEST", result);
    }

    [Fact]
    public void DisplayProperty_UsesValueFormatter_WhenNotListingProperty() {
        // Arrange
        var item = "test";
        var prop = new PropertyConfig(typeof(string).GetProperty("Length")!, _config, 0) {
            Formatter = (obj, provider) => ((int)obj).ToString("D4")
        };

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("0004", result);
    }

    [Fact]
    public void DisplayProperty_ReturnsValueAsString_WhenNoFormatter() {
        // Arrange
        var item = "test";
        var prop = new PropertyConfig(typeof(string).GetProperty("Length")!, _config, 0);

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("4", result);
    }

    [Fact]
    public void DisplayProperty_ReturnsEnumerableCount_WhenEnumerableProperty() {
        // Arrange
        var item = new { List = new List<int> { 1, 2, 3 } };
        var prop = new PropertyConfig(item.GetType().GetProperty("List")!, _config, 0) {
            IsEnumerable = true
        };

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("3", result);
    }

    [Fact]
    public void DisplayProperty_UsesDisplayedProperty_WhenNoDirectFormatter() {
        // Arrange
        var item = new { Inner = new { Key = 42 } };
        var innerPropInfo = item.Inner.GetType().GetProperty("Key");
        var innerPropConfig = new PropertyConfig(innerPropInfo!, _config, 0);
        var propInfo = item.GetType().GetProperty("Inner");
        var prop = new PropertyConfig(propInfo!, _config, 0) {
            DisplayedProperty = innerPropInfo
        };

        _explorerMock
            .Setup(e => e.GetTable(item.Inner.GetType()))
            .Returns(new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "Models", 0) {
                Properties = { innerPropConfig }
            });

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("42", result);
    }

    [Fact]
    public void DisplayProperty_ReturnsEmptyString_WhenPropValueIsNull() {
        // Arrange
        var item = new { Name = (string?)null };
        var prop = new PropertyConfig(item.GetType().GetProperty("Name")!, _config, 0);

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void DisplayProperty_UsesEnumerableFormatter_WhenEnumerableAndValueProvided() {
        // Arrange
        var item = new { List = new List<int> { 1, 2, 3 } };
        var prop = new PropertyConfig(item.GetType().GetProperty("List")!, _config, 0) {
            IsEnumerable = true,
            EnumerableFormatter = (obj, provider) => string.Join(",", ((IEnumerable<int>)obj))
        };

        // Act
        var result = _tableManager.DisplayProperty(item, prop, item.List);

        // Assert
        Assert.Equal("1,2,3", result);
    }

    [Fact]
    public void DisplayProperty_ReturnsEmptyString_WhenDisplayedPropertyAndInnerConfigIsNull() {
        // Arrange
        var item = new { Inner = new { Key = 42 } };
        var innerPropInfo = item.Inner.GetType().GetProperty("Key");
        var propInfo = item.GetType().GetProperty("Inner");
        var prop = new PropertyConfig(propInfo!, _config, 0) {
            DisplayedProperty = innerPropInfo
        };

        _explorerMock
            .Setup(e => e.GetTable(item.Inner.GetType()))
            .Returns((TableConfig?)null);

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("{ Key = 42 }", result); // Returns the value as string if inner config is null
    }

    [Fact]
    public void DisplayProperty_ReturnsKeyValue_WhenDisplayedPropertyIsNull() {
        // Arrange
        var item = new { Inner = new { Key = 42 } };
        var propInfo = item.GetType().GetProperty("Inner");
        var prop = new PropertyConfig(propInfo!, _config, 0);

        var keyProperty = item.Inner.GetType().GetProperty("Key");

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("{ Key = 42 }", result); // Returns key value as string if DisplayedProperty is null
    }

    [Fact]
    public void DisplayProperty_ReturnsToStringValue_WhenNoKeyOrDisplayedProperty() {
        // Arrange
        var item = new { Inner = new { Name = "Test" } };
        var propInfo = item.GetType().GetProperty("Inner");
        var prop = new PropertyConfig(propInfo!, _config, 0);

        // Act
        var result = _tableManager.DisplayProperty(item, prop);

        // Assert
        Assert.Equal("{ Name = Test }", result); // Returns ToString value of inner property
    }
}