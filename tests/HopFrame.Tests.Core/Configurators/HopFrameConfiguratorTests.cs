using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using HopFrame.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Tests.Core.Configurators;

public class HopFrameConfiguratorTests {
    private class TestRepository : IHopFrameRepository {
        public Task<IEnumerable<object>> LoadPageGenericAsync(int page, int perPage, CancellationToken ct) {
            throw new NotImplementedException();
        }
        public Task<int> CountAsync(CancellationToken ct) {
            throw new NotImplementedException();
        }
        public Task<SearchResult> SearchGenericAsync(string searchTerm, int page, int perPage, CancellationToken ct) {
            throw new NotImplementedException();
        }
        public Task CreateGenericAsync(object entry, CancellationToken ct) {
            throw new NotImplementedException();
        }

        public Task UpdateGenericAsync(object entry, CancellationToken ct) {
            throw new NotImplementedException();
        }

        public Task DeleteGenericAsync(object entry, CancellationToken ct) {
            throw new NotImplementedException();
        }
    }

    private HopFrameConfig CreateConfig()
        => new HopFrameConfig { Tables = new List<TableConfig>() };

    private TableConfig CreateValidTable<TModel>()
        => new TableConfig {
            Identifier = typeof(TModel).Name,
            TableType = typeof(TModel),
            RepositoryType = typeof(TestRepository),
            Route = typeof(TModel).Name.ToLower(),
            DisplayName = typeof(TModel).Name,
            OrderIndex = 0,
            Properties = new List<PropertyConfig> {
                new PropertyConfig {
                    Identifier = "Id",
                    DisplayName = "Id",
                    Type = typeof(int),
                    OrderIndex = 0,
                    PropertyType = PropertyType.Numeric
                }
            }
        };

    // -------------------------------------------------------------
    // AddRepository<TRepository, TModel>
    // -------------------------------------------------------------

    [Fact]
    public void AddRepository_AddsTableToConfig() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddRepository<TestRepository, TestModel>();

        Assert.Single(config.Tables);
        Assert.Equal(typeof(TestModel), config.Tables[0].TableType);
    }

    [Fact]
    public void AddRepository_RegistersRepositoryInServices() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        configurator.AddRepository<TestRepository, TestModel>();

        Assert.Contains(services, d => d.ServiceType == typeof(TestRepository));
    }

    [Fact]
    public void AddRepository_InvokesConfiguratorAction() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        bool invoked = false;

        configurator.AddRepository<TestRepository, TestModel>(_ => { invoked = true; });

        Assert.True(invoked);
    }

    // -------------------------------------------------------------
    // AddTable<TModel>
    // -------------------------------------------------------------

    [Fact]
    public void AddTable_AddsValidTableToConfig() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        var table = CreateValidTable<TestModel>();

        configurator.AddTable<TestModel>(table);

        Assert.Single(config.Tables);
        Assert.Equal(table, config.Tables[0]);
    }

    [Fact]
    public void AddTable_RegistersRepositoryType() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        var table = CreateValidTable<TestModel>();

        configurator.AddTable<TestModel>(table);

        Assert.Contains(services, d => d.ServiceType == typeof(TestRepository));
    }

    [Fact]
    public void AddTable_Throws_WhenTableTypeDoesNotMatch() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        var table = CreateValidTable<TestModel>();
        table.TableType = typeof(string); // falscher Typ

        var ex = Assert.Throws<ArgumentException>(() =>
            configurator.AddTable<TestModel>(table));

        Assert.Contains("does not mach requested type", ex.Message);
    }

    [Fact]
    public void AddTable_Throws_WhenValidationFails() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        var table = CreateValidTable<TestModel>();
        table.DisplayName = null!; // invalid

        var ex = Assert.Throws<ArgumentException>(() =>
            configurator.AddTable<TestModel>(table));

        Assert.Contains("validation errors", ex.Message);
        Assert.Contains("DisplayName cannot be null", ex.Message);
    }

    [Fact]
    public void AddTable_InvokesConfiguratorAction() {
        var config = CreateConfig();
        var services = new ServiceCollection();
        var configurator = new HopFrameConfigurator(config, services);

        var table = CreateValidTable<TestModel>();

        bool invoked = false;

        configurator.AddTable<TestModel>(table, _ => { invoked = true; });

        Assert.True(invoked);
    }
}