using System.Linq.Expressions;
using HopFrame.Core.Config;
using HopFrame.Core.Tests.Models;

namespace HopFrame.Core.Tests.Config;

public class PropertyConfiguratorTests {
    [Fact]
    public void SetDisplayName_SetsNameProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        var displayName = "ID";

        // Act
        configurator.SetDisplayName(displayName);

        // Assert
        Assert.Equal(displayName, propertyConfig.Name);
    }

    [Fact]
    public void List_SetsListAndSearchableProperties() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.List(true);

        // Assert
        Assert.True(propertyConfig.List);
        Assert.False(propertyConfig.Searchable);
    }

    [Fact]
    public void IsSortable_SetsSortableProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.IsSortable(true);

        // Assert
        Assert.True(propertyConfig.Sortable);
    }

    [Fact]
    public void IsSearchable_SetsSearchableProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.IsSearchable(true);

        // Assert
        Assert.True(propertyConfig.Searchable);
    }

    [Fact]
    public void SetDisplayedProperty_SetsDisplayedProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<MockModel>(propertyConfig);
        Expression<Func<MockModel, int>> propertyExpression = model => model.Id;

        // Act
        configurator.SetDisplayedProperty(propertyExpression);

        // Assert
        Assert.NotNull(propertyConfig.DisplayedProperty);
        Assert.Equal("Id", propertyConfig.DisplayedProperty?.Name);
    }

    [Fact]
    public void Format_SetsFormatter() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        Func<int, IServiceProvider, string> formatter = (val, _) => val.ToString();

        // Act
        configurator.Format(formatter);

        // Assert
        Assert.NotNull(propertyConfig.Formatter);
    }

    [Fact]
    public void SetParser_SetsParser() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        Func<string, IServiceProvider, int> parser = (str, _) => int.Parse(str);

        // Act
        configurator.SetParser(parser);

        // Assert
        Assert.NotNull(propertyConfig.Parser);
    }

    [Fact]
    public void SetEditable_SetsEditableProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.SetEditable(true);

        // Assert
        Assert.True(propertyConfig.Editable);
    }

    [Fact]
    public void SetCreatable_SetsCreatableProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.SetCreatable(true);

        // Assert
        Assert.True(propertyConfig.Creatable);
    }

    [Fact]
    public void DisplayValue_SetsDisplayValueProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.DisplayValue(true);

        // Assert
        Assert.True(propertyConfig.DisplayValue);
    }

    [Fact]
    public void IsTextArea_SetsTextAreaProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);

        // Act
        configurator.IsTextArea(true);

        // Assert
        Assert.True(propertyConfig.TextArea);
    }

    [Fact]
    public void SetTextAreaRows_SetsTextAreaRowsProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        var rows = 10;

        // Act
        configurator.SetTextAreaRows(rows);

        // Assert
        Assert.Equal(rows, propertyConfig.TextAreaRows);
    }

    [Fact]
    public void SetValidator_SetsValidator() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        Func<int, IServiceProvider, IEnumerable<string>> validator = (_, _) => new List<string>();

        // Act
        configurator.SetValidator(validator);

        // Assert
        Assert.NotNull(propertyConfig.Validator);
    }

    [Fact]
    public void SetOrderIndex_SetsOrderProperty() {
        // Arrange
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!,
            new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0), 0);
        var configurator = new PropertyConfigurator<int>(propertyConfig);
        var orderIndex = 1;

        // Act
        configurator.SetOrderIndex(orderIndex);

        // Assert
        Assert.Equal(orderIndex, propertyConfig.Order);
    }

    [Fact]
    public void Constructor_SetsTableProperty() {
        // Arrange
        var tableConfig = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "MockModels", 0);
        
        // Act
        var propertyConfig = new PropertyConfig(typeof(MockModel).GetProperty("Id")!, tableConfig, 0);
        
        // Assert
        Assert.NotNull(propertyConfig.Table);
        Assert.Equal(tableConfig, propertyConfig.Table);
    }
}