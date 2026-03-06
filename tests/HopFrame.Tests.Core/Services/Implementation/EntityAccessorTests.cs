using HopFrame.Core.Configuration;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementation;
using Moq;

namespace HopFrame.Tests.Core.Services.Implementation;

public class EntityAccessorTests {
    private class TestModel {
        public int Number { get; set; }
        public string? Name { get; set; }
    }

    private PropertyConfig CreateProperty(
        string id,
        Type type,
        Func<object, object?>? getter = null,
        Action<object, object?>? setter = null,
        PropertyType pType = PropertyType.Text) {
        return new PropertyConfig {
            Identifier = id,
            DisplayName = id,
            Type = type,
            PropertyType = pType,
            Table = new TableConfig {
                TableType = typeof(TestModel),
                Properties = [],
                Identifier = string.Empty,
                RepositoryType = typeof(object),
                Route = string.Empty,
                DisplayName = string.Empty
            },
            Getter = getter,
            Setter = setter
        };
    }

    private EntityAccessor CreateAccessor() {
        var mock = new Mock<IConfigAccessor>();
        return new EntityAccessor(mock.Object);
    }

    // -------------------------------------------------------------
    // GetValueRaw
    // -------------------------------------------------------------

    [Fact]
    public void GetValueRaw_UsesGetter_WhenGetterIsSet() {
        var model = new TestModel { Number = 5 };
        var property = CreateProperty("Number", typeof(int), getter: o => 99);

        var accessor = CreateAccessor();

        var result = accessor.GetValueRaw(model, property);

        Assert.Equal(99, result);
    }

    [Fact]
    public void GetValueRaw_ReadsProperty_WhenNoGetterIsSet() {
        var model = new TestModel { Number = 42 };
        var property = CreateProperty("Number", typeof(int));

        var accessor = CreateAccessor();

        var result = accessor.GetValueRaw(model, property);

        Assert.Equal(42, result);
    }

    // -------------------------------------------------------------
    // GetValue
    // -------------------------------------------------------------

    [Fact]
    public void GetValue_ReturnsNull_WhenRawValueIsNull() {
        var model = new TestModel { Name = null };
        var property = CreateProperty("Name", typeof(string));

        var accessor = CreateAccessor();

        var result = accessor.GetValue(model, property);

        Assert.Null(result);
    }

    [Fact]
    public void GetValue_FormatsValue() {
        var model = new TestModel { Name = "Test" };
        var property = CreateProperty("Name", typeof(string));

        var accessor = CreateAccessor();

        var result = accessor.GetValue(model, property);

        Assert.Equal("Test", result);
    }

    // -------------------------------------------------------------
    // FormatValue
    // -------------------------------------------------------------

    [Fact]
    public void FormatValue_ReturnsCount_ForListProperty() {
        var property = CreateProperty("X", typeof(List<object>), pType: PropertyType.List);
        var accessor = CreateAccessor();

        var result = accessor.FormatValue(new List<object> { 1, 2, 3 }, property);

        Assert.Equal("3", result);
    }

    [Fact]
    public void FormatValue_ReturnsToString_ForNormalValue() {
        var property = CreateProperty("X", typeof(int));
        var accessor = CreateAccessor();

        var result = accessor.FormatValue(123, property);

        Assert.Equal("123", result);
    }

    // -------------------------------------------------------------
    // SetValue
    // -------------------------------------------------------------

    [Fact]
    public void SetValue_UsesSetter_WhenSetterIsSet() {
        var model = new TestModel();
        object? receivedModel = null;
        object? receivedValue = null;

        var property = CreateProperty(
            "Number",
            typeof(int),
            setter: (m, v) => {
                receivedModel = m;
                receivedValue = v;
            }
        );

        var accessor = CreateAccessor();

        accessor.SetValue(model, property, 55);

        Assert.Equal(model, receivedModel);
        Assert.Equal(55, receivedValue);
    }

    [Fact]
    public void SetValue_SetsProperty_WhenNoSetterIsSet() {
        var model = new TestModel();
        var property = CreateProperty("Number", typeof(int));

        var accessor = CreateAccessor();

        accessor.SetValue(model, property, 77);

        Assert.Equal(77, model.Number);
    }

    // -------------------------------------------------------------
    // SortDataByProperty
    // -------------------------------------------------------------

    [Fact]
    public void SortDataByProperty_SortsByNormalProperty() {
        var data = new List<TestModel> {
            new() { Number = 3 },
            new() { Number = 1 },
            new() { Number = 2 }
        };

        var property = CreateProperty("Number", typeof(int));

        var accessor = CreateAccessor();

        var result = accessor.SortDataByProperty(data, property).Cast<TestModel>().ToList();

        Assert.Equal([1, 2, 3], result.Select(x => x.Number));
    }

    [Fact]
    public void SortDataByProperty_SortsUsingGetter() {
        var data = new List<TestModel> {
            new() { Number = 3 },
            new() { Number = 1 },
            new() { Number = 2 }
        };

        var property = CreateProperty(
            "Number",
            typeof(int),
            getter: o => ((TestModel)o).Number * -1
        );

        var accessor = CreateAccessor();

        var result = accessor.SortDataByProperty(data, property).Cast<TestModel>().ToList();

        // Sortiert nach -Number → Reihenfolge: 3, 2, 1
        Assert.Equal([3, 2, 1], result.Select(x => x.Number));
    }
}