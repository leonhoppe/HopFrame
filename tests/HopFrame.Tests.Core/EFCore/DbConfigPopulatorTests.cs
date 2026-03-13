using HopFrame.Core.Configuration;
using HopFrame.Core.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Tests.Core.EFCore;

public class DbConfigPopulatorTests {
    private class TestContext { }

    private class TestModel {
        public int Id { get; set; }
    }

    private class OtherModel {
        public int X { get; set; }
    }

    private HopFrameConfig CreateConfig(params TableConfig[] tables)
        => new HopFrameConfig { Tables = tables.ToList() };

    private TableConfig CreateTable(Type modelType)
        => new TableConfig {
            Identifier = modelType.Name,
            TableType = modelType,
            RepositoryType = typeof(object),
            Route = modelType.Name.ToLower(),
            DisplayName = modelType.Name,
            OrderIndex = 0,
            Properties = new List<PropertyConfig> {
                new PropertyConfig {
                    Identifier = "Id",
                    DisplayName = "Id",
                    Type = typeof(int),
                    PropertyType = PropertyType.Numeric
                }
            }
        };

    // -------------------------------------------------------------
    // ConfigureRepository
    // -------------------------------------------------------------

    [Fact]
    public void ConfigureRepository_RegistersRepositoryType() {
        var services = new ServiceCollection();
        var global = CreateConfig();

        var prop = typeof(TestDbContext).GetProperty(nameof(TestDbContext.TestModels))!;

        var identifier = DbConfigPopulator.ConfigureRepository(global, services, typeof(TestDbContext), prop);

        var repoType = typeof(EfCoreRepository<TestModel, TestDbContext>);

        Assert.Contains(services, d => d.ServiceType == repoType);
        Assert.Single(global.Tables);
        Assert.Equal(identifier, global.Tables[0].Identifier);
    }

    [Fact]
    public void ConfigureRepository_AddsTableToGlobalConfig() {
        var services = new ServiceCollection();
        var global = CreateConfig();

        var prop = typeof(TestDbContext).GetProperty(nameof(TestDbContext.TestModels))!;

        DbConfigPopulator.ConfigureRepository(global, services, typeof(TestDbContext), prop);

        Assert.Single(global.Tables);
        Assert.Equal(typeof(TestModel), global.Tables[0].TableType);
    }

    private class TestDbContext : DbContext {
        public List<TestModel> TestModels { get; set; } = new();
    }

    // -------------------------------------------------------------
    // CheckForRelations
    // -------------------------------------------------------------

    private class RelationModel {
        public OtherModel Single { get; set; } = new();
        public List<OtherModel> Many { get; set; } = new();
    }

    [Fact]
    public void CheckForRelations_SetsRelationFlag_ForSingleReference() {
        var otherTable = CreateTable(typeof(OtherModel));
        var relationTable = CreateTable(typeof(RelationModel));

        relationTable.Properties = new List<PropertyConfig> {
            new PropertyConfig {
                Identifier = "Single",
                DisplayName = "Single",
                Type = typeof(OtherModel),
                PropertyType = PropertyType.Text
            }
        };

        var global = CreateConfig(otherTable, relationTable);

        DbConfigPopulator.CheckForRelations(global, relationTable);

        Assert.True((relationTable.Properties[0].PropertyType & PropertyType.Relation) != 0);
        Assert.Equal(typeof(OtherModel), relationTable.Properties[0].RelationType);
    }

    [Fact]
    public void CheckForRelations_SetsRelationFlag_ForListReference() {
        var otherTable = CreateTable(typeof(OtherModel));
        var relationTable = CreateTable(typeof(RelationModel));

        relationTable.Properties = new List<PropertyConfig> {
            new PropertyConfig {
                Identifier = "Many",
                DisplayName = "Many",
                Type = typeof(List<OtherModel>),
                PropertyType = PropertyType.List
            }
        };

        var global = CreateConfig(otherTable, relationTable);

        DbConfigPopulator.CheckForRelations(global, relationTable);

        Assert.True((relationTable.Properties[0].PropertyType & PropertyType.Relation) != 0);
        Assert.Equal(typeof(OtherModel), relationTable.Properties[0].RelationType);
    }

    [Fact]
    public void CheckForRelations_DoesNotSetRelationFlag_WhenNoMatchingTableExists() {
        var relationTable = CreateTable(typeof(RelationModel));

        relationTable.Properties = new List<PropertyConfig> {
            new PropertyConfig {
                Identifier = "Single",
                DisplayName = "Single",
                Type = typeof(OtherModel),
                PropertyType = PropertyType.Text
            }
        };

        var global = CreateConfig(relationTable);

        DbConfigPopulator.CheckForRelations(global, relationTable);

        Assert.False((relationTable.Properties[0].PropertyType & PropertyType.Relation) != 0);
        Assert.Null(relationTable.Properties[0].RelationType);
    }
}