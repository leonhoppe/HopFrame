using System.ComponentModel;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HopFrame.Tests.Core.Services.Implementation;

public class SortServiceTests {
    private class TestModel {
        public int Number { get; init; }
        public string? Name { get; init; }
        public TestModel? Relation { get; set; }
    }

    private TableConfig CreateTable(params PropertyConfig[] properties) {
        var table = new TableConfig {
            Identifier = "TestTable",
            TableType = typeof(TestModel),
            RepositoryType = typeof(object),
            Route = "/test",
            DisplayName = "Test Table",
            Properties = properties.ToList()
        };

        var tableProperty = typeof(PropertyConfig).GetProperty(nameof(PropertyConfig.Table))!;
        foreach (var p in properties) {
            tableProperty.SetValue(p, table);
        }

        return table;
    }

    private PropertyConfig CreateProperty(
        string id,
        Type type,
        PropertyType pType = PropertyType.Text,
        Func<object, object?>? getter = null) {
        return new PropertyConfig {
            Identifier = id,
            DisplayName = id,
            Type = type,
            PropertyType = pType,
            Getter = getter,
            Table = null! // wird in CreateTable gesetzt
        };
    }

    private SortService CreateService(IEntityAccessor? entityAccessor = null, IConfigAccessor? config = null) {
        entityAccessor ??= Mock.Of<IEntityAccessor>();
        config ??= Mock.Of<IConfigAccessor>();
        var logger = NullLogger<SortService>.Instance;
        return new SortService(entityAccessor, config, logger);
    }

    // -------------------------------------------------------------
    // Sort
    // -------------------------------------------------------------

    [Fact]
    public void Sort_ReturnsDataset_WhenPropertyIdentifierIsEmpty() {
        var data = new List<TestModel> {
            new() { Number = 2 },
            new() { Number = 1 }
        }.AsQueryable();

        var table = CreateTable();
        var sorting = new Sorting { PropertyIdentifier = "" };

        var service = CreateService();

        var result = service.Sort(data, sorting, table);

        Assert.Equal(data, result);
    }

    [Fact]
    public void Sort_SortsAscending_ByNormalProperty() {
        var data = new List<TestModel> {
            new() { Number = 5 },
            new() { Number = 1 },
            new() { Number = 3 }
        }.AsQueryable();

        var property = CreateProperty("Number", typeof(int));
        var table = CreateTable(property);

        var sorting = new Sorting {
            PropertyIdentifier = "Number",
            Direction = ListSortDirection.Ascending
        };

        var service = CreateService();

        var result = service.Sort(data, sorting, table).ToList();

        Assert.Equal([1, 3, 5], result.Select(x => x.Number));
    }

    [Fact]
    public void Sort_SortsDescending_ByNormalProperty() {
        var data = new List<TestModel> {
            new() { Number = 1 },
            new() { Number = 3 },
            new() { Number = 2 }
        }.AsQueryable();

        var property = CreateProperty("Number", typeof(int));
        var table = CreateTable(property);

        var sorting = new Sorting {
            PropertyIdentifier = "Number",
            Direction = ListSortDirection.Descending
        };

        var service = CreateService();

        var result = service.Sort(data, sorting, table).ToList();

        Assert.Equal([3, 2, 1], result.Select(x => x.Number));
    }

    // -------------------------------------------------------------
    // Getter-based sorting
    // -------------------------------------------------------------

    [Fact]
    public void Sort_DoesNotSort_WhenGetterIsUsed_BecauseCannotTranslate() {
        var data = new List<TestModel> {
            new() { Number = 1 },
            new() { Number = 2 }
        }.AsQueryable();

        var property = CreateProperty(
            "Number",
            typeof(int),
            getter: _ => 999
        );

        var table = CreateTable(property);

        var sorting = new Sorting {
            PropertyIdentifier = "Number",
            Direction = ListSortDirection.Ascending
        };

        var service = CreateService();

        var result = service.Sort(data, sorting, table);

        Assert.Equal(data, result);
    }

    // -------------------------------------------------------------
    // Relation-based sorting
    // -------------------------------------------------------------

    [Fact]
    public void Sort_DoesNotSort_WhenRelationHasNoPreferredProperty() {
        var data = new List<TestModel> {
            new() { Name = "B" },
            new() { Name = "A" }
        }.AsQueryable();

        var property = CreateProperty("Name", typeof(string), pType: PropertyType.Relation);

        var table = CreateTable(property);

        var config = new Mock<IConfigAccessor>();
        config.Setup(x => x.GetTableByIdentifier(It.IsAny<string>()))
            .Returns((TableConfig?)null);

        var service = CreateService(Mock.Of<IEntityAccessor>(), config.Object);

        var sorting = new Sorting {
            PropertyIdentifier = "Name",
            Direction = ListSortDirection.Ascending
        };

        var result = service.Sort(data, sorting, table);

        Assert.Equal(data, result);
    }

    [Fact]
    public void Sort_UsesPreferredProperty_WhenRelationHasPreferredProperty() {
        var data = new List<TestModel> {
            new() { Name = "C", Relation = new() { Name = "C" }},
            new() { Name = "A", Relation = new() { Name = "A" }},
            new() { Name = "B", Relation = new() { Name = "B" }}
        }.AsQueryable();

        var relationTable = new TableConfig {
            Identifier = "RelationTable",
            TableType = typeof(TestModel),
            RepositoryType = typeof(object),
            Route = "/relation",
            DisplayName = "Relation",
            PreferredProperty = "Name",
        };

        var relationProperty = new PropertyConfig {
            Identifier = "Name",
            DisplayName = "Name",
            Type = typeof(string),
            PropertyType = PropertyType.Text,
            Table = relationTable
        };
        
        relationTable.Properties.Add(relationProperty);

        var property = CreateProperty(nameof(TestModel.Relation), typeof(TestModel), pType: PropertyType.Relation);
        property.RelationTable = relationTable.Identifier;

        var table = CreateTable(property);

        var config = new Mock<IConfigAccessor>();
        config.Setup(x => x.GetTableByIdentifier(relationTable.Identifier))
            .Returns(relationTable);

        var service = CreateService(Mock.Of<IEntityAccessor>(), config.Object);

        var sorting = new Sorting {
            PropertyIdentifier = property.Identifier,
            Direction = ListSortDirection.Ascending
        };

        var result = service.Sort(data, sorting, table).ToList();

        Assert.Equal(["A", "B", "C"], result.Select(x => x.Name));
    }
}