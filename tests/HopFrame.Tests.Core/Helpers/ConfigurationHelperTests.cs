using System.Reflection;
using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace HopFrame.Tests.Core.Helpers;

public class ConfigurationHelperTests {
    private HopFrameConfig CreateGlobal(params TableConfig[] tables)
        => new HopFrameConfig { Tables = tables.ToList() };

    private TableConfig CreateValidTable()
        => new TableConfig {
            Identifier = "Test",
            TableType = typeof(string),
            RepositoryType = typeof(string),
            Route = "/test",
            DisplayName = "Test Table",
            Properties = new List<PropertyConfig> {
                new PropertyConfig {
                    Identifier = "Prop1",
                    DisplayName = "Property 1",
                    Type = typeof(int)
                }
            }
        };

    private TableConfig CreateDummyTable(string identifier)
        => new TableConfig {
            Identifier = identifier,
            TableType = typeof(object),
            RepositoryType = typeof(object),
            Route = identifier.ToLower(),
            DisplayName = identifier,
            OrderIndex = 0
        };

    [Fact]
    public void InitializeTable_UsesModelNameAsIdentifier_WhenUnique() {
        var global = CreateGlobal();

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.Equal("TestModel", config.Identifier);
    }

    [Fact]
    public void InitializeTable_GeneratesGuid_WhenIdentifierAlreadyExists() {
        var existing = CreateDummyTable("TestModel");
        var global = CreateGlobal(existing);

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.NotEqual("TestModel", config.Identifier);
        Assert.True(Guid.TryParse(config.Identifier, out _));
    }

    [Fact]
    public void InitializeTable_SetsBasicFieldsCorrectly() {
        var global = CreateGlobal();

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.Equal(typeof(string), config.RepositoryType);
        Assert.Equal(typeof(TestModel), config.TableType);
        Assert.Equal("testmodel", config.Route);
        Assert.Equal("TestModel", config.DisplayName);
        Assert.Equal(0, config.OrderIndex);
    }

    [Fact]
    public void InitializeTable_SetsOrderIndex_ToCurrentTableCount() {
        var global = CreateGlobal(
            CreateDummyTable("A"),
            CreateDummyTable("B")
        );

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.Equal(2, config.OrderIndex);
    }

    [Fact]
    public void InitializeTable_CreatesPropertyConfigs_ForAllModelProperties() {
        var global = CreateGlobal();

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.Equal(2, config.Properties.Count);
        Assert.Contains(config.Properties, p => p.Identifier == "Id");
        Assert.Contains(config.Properties, p => p.Identifier == "Name");
    }

    [Fact]
    public void InitializeProperty_UsesPropertyNameAsIdentifier_WhenUnique() {
        var table = CreateDummyTable("T");
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Id))!;

        var config = InvokeInitializeProperty(table, property);

        Assert.Equal("Id", config.Identifier);
    }

    [Fact]
    public void InitializeProperty_GeneratesGuid_WhenIdentifierAlreadyExists() {
        var table = CreateDummyTable("T");
        table.Properties.Add(new PropertyConfig
            { Identifier = "Id", Type = typeof(int), DisplayName = "Id", OrderIndex = 0 });

        var property = typeof(TestModel).GetProperty(nameof(TestModel.Id))!;

        var config = InvokeInitializeProperty(table, property);

        Assert.NotEqual("Id", config.Identifier);
        Assert.True(Guid.TryParse(config.Identifier, out _));
    }

    [Fact]
    public void InitializeProperty_SetsBasicFieldsCorrectly() {
        var table = CreateDummyTable("T");
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;

        var config = InvokeInitializeProperty(table, property);

        Assert.Equal("Name", config.DisplayName);
        Assert.Equal(typeof(string), config.Type);
        Assert.Equal(0, config.OrderIndex);
    }

    [Fact]
    public void InitializeProperty_SetsOrderIndex_ToCurrentPropertyCount() {
        var table = CreateDummyTable("T");
        table.Properties.Add(new PropertyConfig
            { Identifier = "X", Type = typeof(int), DisplayName = "X", OrderIndex = 0 });

        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;

        var config = InvokeInitializeProperty(table, property);

        Assert.Equal(1, config.OrderIndex);
    }

    private PropertyConfig InvokeInitializeProperty(TableConfig table, PropertyInfo property) {
        var method = typeof(ConfigurationHelper)
            .GetMethod("InitializeProperty", BindingFlags.NonPublic | BindingFlags.Static)!;

        return (PropertyConfig)method.Invoke(null, [table, property])!;
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenIdentifierNotUnique() {
        var config = CreateValidTable();
        var global = CreateGlobal(new TableConfig {
            Identifier = "Test",
            DisplayName = null,
            TableType = null,
            RepositoryType = null,
            Route = null
        });

        var result = ConfigurationHelper.ValidateTable(global, config).ToList();

        Assert.Contains("Table identifier 'Test' is not unique", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenTableTypeIsNull() {
        var config = CreateValidTable();
        config.TableType = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("TableType cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenRepositoryTypeIsNull() {
        var config = CreateValidTable();
        config.RepositoryType = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("RepositoryType cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenRouteIsNull() {
        var config = CreateValidTable();
        config.Route = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("Route cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenDisplayNameIsNull() {
        var config = CreateValidTable();
        config.DisplayName = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("DisplayName cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenPropertyIdentifierNotUnique() {
        var config = CreateValidTable();
        config.Properties.Add(new PropertyConfig {
            Identifier = "Prop1",
            DisplayName = "Duplicate",
            Type = typeof(int)
        });

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("Property identifier 'Prop1' is not unique", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenPropertyDisplayNameIsNull() {
        var config = CreateValidTable();
        config.Properties[0].DisplayName = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("Property 'Prop1': DisplayName cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsError_WhenPropertyTypeIsNull() {
        var config = CreateValidTable();
        config.Properties[0].Type = null;

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Contains("Property 'Prop1': Type cannot be null", result);
    }

    [Fact]
    public void ValidateTable_ReturnsNoErrors_WhenConfigIsValid() {
        var config = CreateValidTable();

        var result = ConfigurationHelper.ValidateTable(CreateGlobal(), config).ToList();

        Assert.Empty(result);
    }
}