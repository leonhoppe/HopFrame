using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Web.Helpers;

namespace HopFrame.Web.Services.Implementation;

public sealed class SearchSuggestionProvider : ISearchSuggestionProvider {
    
    public IEnumerable<string> GenerateSearchSuggestions(TableConfig table, string searchText) {
        var searchParts = searchText.Trim().Split(' ');
        if (searchParts.Length != 0 && searchParts.Last().EndsWith('=') && !searchText.EndsWith(' ')) {
            var part = searchParts.Last()[..^1];
            var property = table.Properties
                .Where(p => p.List)
                .Where(p => !p.IsVirtualProperty)
                .FirstOrDefault(p => p.Name == part);
            
            if (property is null) return [];

            if (property.Info.PropertyType.IsEnum)
                return Enum.GetNames(property.Info.PropertyType);

            if (property.Info.PropertyType == typeof(DateOnly))
                return [DateOnly.FromDateTime(DateTime.Now).ToString()];

            if (property.Info.PropertyType == typeof(TimeOnly))
                return [TimeOnly.FromDateTime(DateTime.Now).ToString()];
        }

        if (searchText.Length != 0 && !searchText.EndsWith(' '))
            return [];

        Type[] validTypes = [typeof(string), typeof(Guid), typeof(bool), typeof(DateOnly), typeof(TimeOnly)];
        var searchableProperties = table.Properties
            .Where(p => !p.IsVirtualProperty)
            .Where(p => p.List)
            .Where(p => p.Searchable)
            .Where(p => 
                p.Info.PropertyType.IsEnum || 
                p.Info.PropertyType.IsNumeric() ||
                validTypes.Contains(p.Info.PropertyType) ||
                p.IsRelation)
            .ToArray();

        return searchableProperties
            .Select(p => p.Name + "=");
    }
    
    public string CompleteSearchSuggestion(TableConfig table, string searchText, string selectedSuggestion) {
        return searchText + selectedSuggestion;
    }
    
}