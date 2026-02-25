using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HopFrame.Tests.Core.Services.Implementation;

public class ConfigAccessorTests {
    private class TestRepository : IHopFrameRepository {
        public Task<IEnumerable<object>> LoadPageGenericAsync(int page, int perPage, CancellationToken ct) {
            throw new NotImplementedException();
        }
        public Task<int> CountAsync(CancellationToken ct) {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<object>> SearchGenericAsync(string searchTerm, int page, int perPage, CancellationToken ct) {
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
    
    private TableConfig CreateTable(string id, string route, Type type)
        => new TableConfig {
            Identifier = id,
            Route = route,
            TableType = type,
            RepositoryType = typeof(TestRepository),
            DisplayName = id,
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

    private HopFrameConfig CreateConfig(params TableConfig[] tables)
        => new HopFrameConfig { Tables = new List<TableConfig>(tables) };

    // -------------------------------------------------------------
    // GetTableByIdentifier
    // -------------------------------------------------------------

    [Fact]
    public void GetTableByIdentifier_ReturnsCorrectTable() {
        var table = CreateTable("A", "a", typeof(TestModel));
        var config = CreateConfig(table);

        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByIdentifier("A");

        Assert.Equal(table, result);
    }

    [Fact]
    public void GetTableByIdentifier_ReturnsNull_WhenNotFound() {
        var config = CreateConfig();
        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByIdentifier("missing");

        Assert.Null(result);
    }

    // -------------------------------------------------------------
    // GetTableByRoute
    // -------------------------------------------------------------

    [Fact]
    public void GetTableByRoute_ReturnsCorrectTable() {
        var table = CreateTable("A", "routeA", typeof(TestModel));
        var config = CreateConfig(table);

        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByRoute("routeA");

        Assert.Equal(table, result);
    }

    [Fact]
    public void GetTableByRoute_ReturnsNull_WhenNotFound() {
        var config = CreateConfig();
        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByRoute("missing");

        Assert.Null(result);
    }

    // -------------------------------------------------------------
    // GetTableByType
    // -------------------------------------------------------------

    [Fact]
    public void GetTableByType_ReturnsCorrectTable() {
        var table = CreateTable("A", "a", typeof(TestModel));
        var config = CreateConfig(table);

        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByType(typeof(TestModel));

        Assert.Equal(table, result);
    }

    [Fact]
    public void GetTableByType_ReturnsNull_WhenNotFound() {
        var config = CreateConfig();
        var accessor = new ConfigAccessor(config, Mock.Of<IServiceProvider>());

        var result = accessor.GetTableByType(typeof(TestModel));

        Assert.Null(result);
    }

    // -------------------------------------------------------------
    // LoadRepository
    // -------------------------------------------------------------

    [Fact]
    public void LoadRepository_ResolvesRepositoryFromServiceProvider() {
        var table = CreateTable("A", "a", typeof(TestModel));

        var repo = new TestRepository();

        var providerMock = new Mock<IServiceProvider>();
        providerMock
            .Setup(p => p.GetService(typeof(TestRepository)))
            .Returns(repo);

        var accessor = new ConfigAccessor(CreateConfig(table), providerMock.Object);

        var result = accessor.LoadRepository(table);

        Assert.Equal(repo, result);
    }

    [Fact]
    public void LoadRepository_Throws_WhenServiceNotRegistered() {
        var table = CreateTable("A", "a", typeof(TestModel));

        var provider = new ServiceCollection().BuildServiceProvider();

        var accessor = new ConfigAccessor(CreateConfig(table), provider);

        Assert.Throws<InvalidOperationException>(() => accessor.LoadRepository(table));
    }
}