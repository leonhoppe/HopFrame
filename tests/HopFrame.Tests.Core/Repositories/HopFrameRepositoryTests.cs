using HopFrame.Core.Repositories;
using Moq;

namespace HopFrame.Tests.Core.Repositories;

public class HopFrameRepositoryTests {
    private Mock<HopFrameRepository<TestModel>> CreateMock()
        => new(MockBehavior.Strict);

    // -------------------------------------------------------------
    // LoadPageGenericAsync
    // -------------------------------------------------------------

    [Fact]
    public async Task LoadPageGenericAsync_DelegatesToTypedMethod() {
        var mock = CreateMock();

        var expected = new List<TestModel> { new TestModel { Id = 1 } };

        mock.Setup(r => r.LoadPageAsync(2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await mock.Object.LoadPageGenericAsync(2, 10, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    // -------------------------------------------------------------
    // SearchGenericAsync
    // -------------------------------------------------------------

    [Fact]
    public async Task SearchGenericAsync_DelegatesToTypedMethod() {
        var mock = CreateMock();

        var expected = new List<TestModel> { new TestModel { Id = 5 } };

        mock.Setup(r => r.SearchAsync("abc", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await mock.Object.SearchGenericAsync("abc", 1, 20, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    // -------------------------------------------------------------
    // CreateGenericAsync
    // -------------------------------------------------------------

    [Fact]
    public async Task CreateGenericAsync_CastsAndDelegates() {
        var mock = CreateMock();

        var model = new TestModel { Id = 99 };

        mock.Setup(r => r.CreateAsync(model, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await mock.Object.CreateGenericAsync(model, CancellationToken.None);

        mock.Verify(r => r.CreateAsync(model, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------
    // UpdateGenericAsync
    // -------------------------------------------------------------

    [Fact]
    public async Task UpdateGenericAsync_CastsAndDelegates() {
        var mock = CreateMock();

        var model = new TestModel { Id = 77 };

        mock.Setup(r => r.UpdateAsync(model, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await mock.Object.UpdateGenericAsync(model, CancellationToken.None);

        mock.Verify(r => r.UpdateAsync(model, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------
    // DeleteGenericAsync
    // -------------------------------------------------------------

    [Fact]
    public async Task DeleteGenericAsync_CastsAndDelegates() {
        var mock = CreateMock();

        var model = new TestModel { Id = 42 };

        mock.Setup(r => r.DeleteAsync(model, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await mock.Object.DeleteGenericAsync(model, CancellationToken.None);

        mock.Verify(r => r.DeleteAsync(model, It.IsAny<CancellationToken>()), Times.Once);
    }

    // -------------------------------------------------------------
    // CountAsync (direct abstract method)
    // -------------------------------------------------------------

    [Fact]
    public async Task CountAsync_CanBeMockedAndReturnsValue() {
        var mock = CreateMock();

        mock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);

        var result = await mock.Object.CountAsync();

        Assert.Equal(123, result);
    }
}