using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services;

/// A service used to modify the actual properties of a model
public interface IEntityAccessor {

    /// <summary>
    /// Returns the formatted content of the property, ready to be displayed
    /// </summary>
    /// <param name="model">The model to pull the property from</param>
    /// <param name="property">The property that shall be extracted</param>
    public string? GetValue(object model, PropertyConfig property);

    /// <summary>
    /// Formats the property to be displayed properly
    /// </summary>
    /// <param name="value">The value of the property</param>
    /// <param name="property">The property that shall be extracted</param>
    public string? FormatValue(object? value, PropertyConfig property);
    
    /// <summary>
    /// Properly formats and sets the new value of the property
    /// </summary>
    /// <param name="model">The model to save the property to</param>
    /// <param name="property">The property that shall be modified</param>
    /// <param name="value">The new value of the property</param>
    public void SetValue(object model, PropertyConfig property, object value);

    /// <summary>
    /// Sorts the provided dataset by the specified property
    /// </summary>
    /// <param name="data">The dataset that needs to be sorted</param>
    /// <param name="property">The property that defines the sort order</param>
    /// <param name="descending">Determines if the resulting order should be flipped</param>
    public IEnumerable<object> SortDataByProperty(IEnumerable<object> data, PropertyConfig property, bool descending = false);

}