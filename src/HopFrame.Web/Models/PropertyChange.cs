using System.Reflection;

namespace HopFrame.Web.Models;

public class PropertyChange(PropertyInfo info, object? value) {
    public object? Value { get; set; } = value;
    public PropertyInfo Property { get; set; } = info;
}
