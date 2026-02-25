using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services;

/// A service used to modify the actual properties of a model
public interface IEntityAccessor {

    /// <summary>
    /// Returns the formatted content of the property, ready to be displayed
    /// </summary>
    /// <param name="model">The model to pull the property from</param>
    /// <param name="property">The property that shall be extracted</param>
    public Task<string?> GetValue(object model, PropertyConfig property);
    
    /// <summary>
    /// Properly formats and sets the new value of the property
    /// </summary>
    /// <param name="model">The model to save the property to</param>
    /// <param name="property">The property that shall be modified</param>
    /// <param name="value">The new value of the property</param>
    public Task SetValue(object model, PropertyConfig property, object value);

}