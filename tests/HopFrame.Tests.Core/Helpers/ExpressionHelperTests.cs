using System.Linq.Expressions;
using HopFrame.Core.Helpers;

namespace HopFrame.Tests.Core.Helpers;

public class ExpressionHelperTests {

    [Fact]
    public void GetPropertyInfo_ReturnsPropertyInfo_WhenExpressionIsValid() {
        Expression<Func<TestModel, int>> expr = x => x.Id;

        var prop = ExpressionHelper.GetPropertyInfo(expr);

        Assert.Equal(nameof(TestModel.Id), prop.Name);
        Assert.Equal(typeof(int), prop.PropertyType);
    }

    [Fact]
    public void GetPropertyInfo_Throws_WhenExpressionRefersToMethod() {
        Expression<Func<TestModel, int>> expr = x => x.Method();

        var ex = Assert.Throws<ArgumentException>(() => ExpressionHelper.GetPropertyInfo(expr));

        Assert.Contains("refers to a method", ex.Message);
    }

    [Fact]
    public void GetPropertyInfo_Throws_WhenExpressionRefersToField() {
        Expression<Func<TestModel, int>> expr = x => x.FieldBacking;

        var ex = Assert.Throws<ArgumentException>(() => ExpressionHelper.GetPropertyInfo(expr));

        Assert.Contains("refers to a field", ex.Message);
    }

    [Fact]
    public void GetPropertyInfo_Throws_WhenExpressionBodyIsNotMemberExpression() {
        Expression<Func<TestModel, int>> expr = x => 5;

        var ex = Assert.Throws<ArgumentException>(() => ExpressionHelper.GetPropertyInfo(expr));

        Assert.Contains("refers to a method, not a property", ex.Message);
    }
}