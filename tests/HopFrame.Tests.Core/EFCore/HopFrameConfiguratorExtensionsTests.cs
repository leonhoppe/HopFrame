using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using HopFrame.Core.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Tests.Core.EFCore;

public class HopFrameConfiguratorExtensionsTests {
    private class TestModelA {
        public int Id { get; set; }
    }

    private class TestModelB {
        public int Id { get; set; }
    }

    private class TestDbContext : DbContext {
        public DbSet<TestModelA> A { get; set; } = null!;
        public DbSet<TestModelB> B { get; set; } = null!;
    }

    private HopFrameConfig CreateConfig()
        => new HopFrameConfig { Tables = new List<TableConfig>() };

    // -------------------------------------------------------------
    // AddDbContext - all tables
    // -------------------------------------------------------------

    [Fact]
    public void AddDbContext_AddsAllDbSets_WhenNoFilterProvided() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddDbContext<TestDbContext>();

        Assert.Equal(2, config.Tables.Count);
        Assert.Contains(config.Tables, t => t.TableType == typeof(TestModelA));
        Assert.Contains(config.Tables, t => t.TableType == typeof(TestModelB));
    }

    [Fact]
    public void AddDbContext_RegistersRepositoriesForAllDbSets() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddDbContext<TestDbContext>();

        Assert.Contains(services, d => d.ServiceType == typeof(EfCoreRepository<TestModelA, TestDbContext>));
        Assert.Contains(services, d => d.ServiceType == typeof(EfCoreRepository<TestModelB, TestDbContext>));
    }

    // -------------------------------------------------------------
    // AddDbContext - filtered tables
    // -------------------------------------------------------------

    [Fact]
    public void AddDbContext_UsesOnlyIncludedTables_WhenFilterProvided() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddDbContext<TestDbContext>(ctx => ctx.A
        );

        Assert.Single(config.Tables);
        Assert.Equal(typeof(TestModelA), config.Tables[0].TableType);
    }

    [Fact]
    public void AddDbContext_RegistersOnlyFilteredRepositories() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddDbContext<TestDbContext>(ctx => ctx.A
        );

        Assert.Contains(services, d => d.ServiceType == typeof(EfCoreRepository<TestModelA, TestDbContext>));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(EfCoreRepository<TestModelB, TestDbContext>));
    }

    // -------------------------------------------------------------
    // Relation detection
    // -------------------------------------------------------------

    private class RelationModel {
        public TestModelA Single { get; set; } = null!;
        public List<TestModelB> Many { get; set; } = new();
    }

    private class RelationDbContext : DbContext {
        public DbSet<TestModelA> A { get; set; } = null!;
        public DbSet<TestModelB> B { get; set; } = null!;
        public DbSet<RelationModel> R { get; set; } = null!;
    }

    [Fact]
    public void AddDbContext_ChecksRelationsForCreatedTables() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddDbContext<RelationDbContext>();

        var relationTable = config.Tables.Single(t => t.TableType == typeof(RelationModel));

        // At least one property should have the Relation flag
        Assert.Contains(relationTable.Properties, p => (p.PropertyType & PropertyType.Relation) != 0);
    }

    // -------------------------------------------------------------
    // Fluent API
    // -------------------------------------------------------------

    [Fact]
    public void AddDbContext_ReturnsConfigurator() {
        var services = new ServiceCollection();
        var config = CreateConfig();
        var configurator = new HopFrameConfigurator(config, services);

        var result = configurator.AddDbContext<TestDbContext>();

        Assert.Same(configurator, result);
    }
}