using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;

namespace HopFrame.Tests.Core.Configurators;

public class PropertyConfiguratorTests {
    private PropertyConfig CreateConfig(PropertyType type)
        => new PropertyConfig {
            Identifier = "Test",
            DisplayName = "Test",
            Type = typeof(string),
            OrderIndex = 0,
            PropertyType = type
        };

    [Fact]
    public void SetType_ReplacesBaseType_AndPreservesModifiers() {
        // Arrange: Nullable + List + Numeric
        var original = PropertyType.Numeric | PropertyType.Nullable | PropertyType.List;
        var config = CreateConfig(original);

        var configurator = new PropertyConfigurator<object, string>(config);

        // Act: change base type to Text
        configurator.SetType(PropertyType.Text);

        // Assert: modifiers remain, base type replaced
        Assert.Equal(
            PropertyType.Text | PropertyType.Nullable | PropertyType.List,
            config.PropertyType
        );
    }

    [Fact]
    public void SetType_DoesNotAffectModifiers_WhenSettingSameBaseType() {
        var original = PropertyType.Boolean | PropertyType.Nullable;
        var config = CreateConfig(original);

        var configurator = new PropertyConfigurator<object, string>(config);

        configurator.SetType(PropertyType.Boolean);

        Assert.Equal(original, config.PropertyType);
    }

    [Fact]
    public void SetType_CanChangeEnumToNumeric_WhileKeepingModifiers() {
        var original = PropertyType.Enum | PropertyType.List;
        var config = CreateConfig(original);

        var configurator = new PropertyConfigurator<object, string>(config);

        configurator.SetType(PropertyType.Numeric);

        Assert.Equal(
            PropertyType.Numeric | PropertyType.List,
            config.PropertyType
        );
    }

    [Fact]
    public void SetType_CanChangeToEmail_AndPreserveNullable() {
        var original = PropertyType.Text | PropertyType.Nullable;
        var config = CreateConfig(original);

        var configurator = new PropertyConfigurator<object, string>(config);

        configurator.SetType(PropertyType.Email);

        Assert.Equal(
            PropertyType.Email | PropertyType.Nullable,
            config.PropertyType
        );
    }

    [Fact]
    public void SetType_ReturnsConfigurator_ForFluentApi() {
        var config = CreateConfig(PropertyType.Text);
        var configurator = new PropertyConfigurator<object, string>(config);

        var result = configurator.SetType(PropertyType.Numeric);

        Assert.Same(configurator, result);
    }
}