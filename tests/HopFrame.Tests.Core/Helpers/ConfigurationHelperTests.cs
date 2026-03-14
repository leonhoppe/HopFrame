using System.ComponentModel.DataAnnotations;
using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace HopFrame.Tests.Core.Helpers;

public class ConfigurationHelperTests {
    private class PropertyTypeModel {
        public int Number { get; set; }
        public int? NullableNumber { get; set; }
        public string Text { get; set; } = "";
        public bool Flag { get; set; }
        public DateTime Timestamp { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public TestEnum EnumValue { get; set; }
        public List<int> Numbers { get; set; } = new();
        public IEnumerable<string> Strings { get; set; } = new List<string>();

        [EmailAddress]
        public string Email { get; set; } = "";
    }

    private enum TestEnum {
        A,
        B
    }


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
                    Type = typeof(int),
                    PropertyType = PropertyType.Numeric
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
        Assert.Equal("testmodels", config.Route);
        Assert.Equal("TestModels", config.DisplayName);
        Assert.Equal(0, config.OrderIndex);
    }

    [Fact]
    public void InitializeTable_SetsOrderIndex_ToCurrentTableCount() {
        var global = CreateGlobal(
            CreateDummyTable("A"),
            CreateDummyTable("B")
        );

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(TestModel));

        Assert.Equal(20, config.OrderIndex);
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
    public void InitializeTable_SetsPropertyTypes_ForAllProperties() {
        var global = CreateGlobal();

        var config = ConfigurationHelper.InitializeTable(global, typeof(string), typeof(PropertyTypeModel));

        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.Numeric);
        Assert.Contains(config.Properties, p => p.PropertyType == (PropertyType.Numeric | PropertyType.Nullable));
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.Boolean);
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.DateTime);
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.DateOnly);
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.TimeOnly);
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.Enum);
        Assert.Contains(config.Properties, p => p.PropertyType == (PropertyType.Numeric | PropertyType.List));
        Assert.Contains(config.Properties, p => p.PropertyType == (PropertyType.Text | PropertyType.List));
        Assert.Contains(config.Properties, p => p.PropertyType == PropertyType.Email);
    }

    [Fact]
    public void InitializeProperty_UsesPropertyNameAsIdentifier_WhenUnique() {
        var table = CreateDummyTable("T");
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Id))!;

        var config = ConfigurationHelper.InitializeProperty(table, property.PropertyType, property.Name, property);

        Assert.Equal("Id", config.Identifier);
    }

    [Fact]
    public void InitializeProperty_GeneratesGuid_WhenIdentifierAlreadyExists() {
        var table = CreateDummyTable("T");
        table.Properties.Add(new PropertyConfig {
            Identifier = "Id", Type = typeof(int), DisplayName = "Id", OrderIndex = 0,
            PropertyType = PropertyType.Numeric
        });

        var property = typeof(TestModel).GetProperty(nameof(TestModel.Id))!;

        var config = ConfigurationHelper.InitializeProperty(table, property.PropertyType, property.Name, property);

        Assert.NotEqual("Id", config.Identifier);
        Assert.True(Guid.TryParse(config.Identifier, out _));
    }

    [Fact]
    public void InitializeProperty_SetsBasicFieldsCorrectly() {
        var table = CreateDummyTable("T");
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;

        var config = ConfigurationHelper.InitializeProperty(table, property.PropertyType, property.Name, property);

        Assert.Equal("Name", config.DisplayName);
        Assert.Equal(typeof(string), config.Type);
        Assert.Equal(0, config.OrderIndex);
    }

    [Fact]
    public void InitializeProperty_SetsOrderIndex_ToCurrentPropertyCount() {
        var table = CreateDummyTable("T");
        table.Properties.Add(new PropertyConfig {
            Identifier = "X", Type = typeof(int), DisplayName = "X", OrderIndex = 0, PropertyType = PropertyType.Numeric
        });

        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;

        var config = ConfigurationHelper.InitializeProperty(table, property.PropertyType, property.Name, property);

        Assert.Equal(1, config.OrderIndex);
    }

    [Fact]
    public void InitializeProperty_SetsPropertyType_FromInferPropertyType() {
        var table = CreateDummyTable("T");
        var property = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Number))!;

        var config = ConfigurationHelper.InitializeProperty(table, property.PropertyType, property.Name, property);

        Assert.Equal(PropertyType.Numeric, config.PropertyType);
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
            Type = typeof(int),
            PropertyType = PropertyType.Numeric
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

    [Fact]
    public void InferPropertyType_RecognizesNumeric() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Number))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(int), prop);

        Assert.Equal(PropertyType.Numeric, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesNullableNumeric() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.NullableNumber))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(int?), prop);

        Assert.Equal(PropertyType.Numeric | PropertyType.Nullable, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesBoolean() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Flag))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(bool), prop);

        Assert.Equal(PropertyType.Boolean, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesDateTime() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Timestamp))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(DateTime), prop);

        Assert.Equal(PropertyType.DateTime, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesDateOnly() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Date))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(DateOnly), prop);

        Assert.Equal(PropertyType.DateOnly, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesTimeOnly() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Time))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(TimeOnly), prop);

        Assert.Equal(PropertyType.TimeOnly, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesEnum() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.EnumValue))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(TestEnum), prop);

        Assert.Equal(PropertyType.Enum, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesList() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Numbers))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(List<int>), prop);

        Assert.Equal(PropertyType.Numeric | PropertyType.List, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesEnumerable() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Strings))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(IEnumerable<string>), prop);

        Assert.Equal(PropertyType.Text | PropertyType.List, result);
    }

    [Fact]
    public void InferPropertyType_RecognizesEmail() {
        var prop = typeof(PropertyTypeModel).GetProperty(nameof(PropertyTypeModel.Email))!;
        var result = ConfigurationHelper.InferPropertyType(typeof(string), prop);

        Assert.Equal(PropertyType.Email, result);
    }
}