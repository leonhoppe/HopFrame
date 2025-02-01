// ReSharper disable GenericEnumeratorNotDisposed

using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementations;
using HopFrame.Tests.Core.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HopFrame.Tests.Core.Services;

public class TableManagerTests {
    private Mock<DbContext> CreateMockDbContext<TModel>(List<TModel> data) where TModel : class {
        var dbContext = new Mock<DbContext>();
        var dbSet = CreateMockDbSet(data);

        dbContext.Setup(m => m.Set<TModel>()).Returns(dbSet.Object);
        dbContext.Setup(m => m.Entry(It.IsAny<MockModel>())).Returns<MockModel>(entry => new MockDbContext().Entry(entry));
        return dbContext;
    }

    private Mock<DbSet<TModel>> CreateMockDbSet<TModel>(List<TModel> data) where TModel : class {
        var queryableData = data.AsQueryable();
        var dbSet = new Mock<DbSet<TModel>>();

        dbSet.As<IQueryable<TModel>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<TModel>(queryableData.Provider));
        dbSet.As<IQueryable<TModel>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        dbSet.As<IQueryable<TModel>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        dbSet.As<IQueryable<TModel>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        dbSet.As<IAsyncEnumerable<TModel>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TModel>(queryableData.GetEnumerator()));
        dbSet.As<IQueryable<TModel>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<TModel>(queryableData.Provider));
        dbSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) =>
            data.FirstOrDefault(d => ids.Contains(d.GetType().GetProperty("Id")!.GetValue(d, null))));

        return dbSet;
    }

    [Fact]
    public void LoadPage_ReturnsPagedData() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" },
            new MockModel { Id = 2, Name = "Item2" },
            new MockModel { Id = 3, Name = "Item3" }
        };
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);

        // Act
        var result = manager.LoadPage(1, 2).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Item3", ((MockModel)result[0]).Name);
    }

    [Fact]
    public async Task Search_ReturnsMatchingData() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" },
            new MockModel { Id = 2, Name = "Item2" },
            new MockModel { Id = 3, Name = "TestItem" }
        };
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        config.Properties.Add(new PropertyConfig(typeof(MockModel).GetProperty("Name")!, config, 0)
            { Searchable = true });
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);

        // Act
        var (result, totalPages) = await manager.Search("Test", 0, 2);

        // Assert
        var collection = result as object[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.Equal("TestItem", ((MockModel)collection.First()).Name);
        Assert.Equal(1, totalPages);
    }

    [Fact]
    public async Task TotalPages_ReturnsCorrectPageCount() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" },
            new MockModel { Id = 2, Name = "Item2" },
            new MockModel { Id = 3, Name = "Item3" }
        };
        var dbContext = new MockDbContext();
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext, config, explorer.Object, provider.Object);
        await dbContext.Models.AddRangeAsync(data);
        await dbContext.SaveChangesAsync();

        // Act
        var totalPages = await manager.TotalPages(2);

        // Assert
        Assert.Equal(2, totalPages);
    }

    [Fact]
    public async Task DeleteItem_RemovesItemFromDbSet() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" },
            new MockModel { Id = 2, Name = "Item2" }
        };
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);
        var item = data.First();

        // Act
        await manager.DeleteItem(item);

        // Assert
        dbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        dbContext.Verify(m => m.Set<MockModel>().Remove(item), Times.Once);
    }

    [Fact]
    public async Task EditItem_SavesChanges() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" }
        };
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);

        // Act
        await manager.EditItem(data.First());

        // Assert
        dbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddItem_AddsItemToDbSet() {
        // Arrange
        var data = new List<MockModel>();
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext), null!), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);
        var newItem = new MockModel { Id = 3, Name = "NewItem" };

        // Act
        await manager.AddItem(newItem);

        // Assert
        dbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        dbContext.Verify(m => m.Set<MockModel>().AddAsync(newItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    /*[Fact]
    public async Task RevertChanges_ReloadsItem() {
        // Arrange
        var data = new List<MockModel> {
            new MockModel { Id = 1, Name = "Item1" }
        };
        var dbContext = CreateMockDbContext(data);
        var config = new TableConfig(new DbContextConfig(typeof(MockDbContext)), typeof(MockModel), "Models", 0);
        var explorer = new Mock<IContextExplorer>();
        var provider = new Mock<IServiceProvider>();
        var manager = new TableManager<MockModel>(dbContext.Object, config, explorer.Object, provider.Object);
        var item = data.First();

        // Act
        await manager.RevertChanges(item);

        // Assert
        dbContext.Verify(m => m.Entry(item), Times.AtLeastOnce);
    }*/
}