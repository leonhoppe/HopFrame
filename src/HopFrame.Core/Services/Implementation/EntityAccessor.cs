using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services.Implementation;

internal class EntityAccessor : IEntityAccessor {
    
    public async Task<string?> GetValue(object model, PropertyConfig property) {
        var prop = model.GetType().GetProperty(property.Identifier);

        if (prop is null)
            return null;

        return prop.GetValue(model)?.ToString();
    }
    
    public async Task SetValue(object model, PropertyConfig property, object value) {
        var prop = model.GetType().GetProperty(property.Identifier);

        if (prop is null)
            return;
        
        prop.SetValue(model, Convert.ChangeType(value, property.Type));
    }
    
}