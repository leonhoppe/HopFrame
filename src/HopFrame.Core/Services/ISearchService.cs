using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;

namespace HopFrame.Core.Services;

/// A service used to filter a dataset by a specific search term
public interface ISearchService {

    /// <summary>
    /// Searches through the provided dataset
    /// </summary>
    /// <param name="dataset">The dataset that should be searched</param>
    /// <param name="table">The type of the dataset</param>
    /// <param name="searchTerm">The term to search for</param>
    /// <typeparam name="TModel">The type of the provided data</typeparam>
    /// <returns>The filtered queryable</returns>
    public IQueryable<TModel> Search<TModel>(IQueryable<TModel> dataset, TableConfig table, string searchTerm) where TModel : notnull;

    /// <summary>
    /// Searches through the provided dataset
    /// </summary>
    /// <param name="dataset">The dataset that should be searched</param>
    /// <param name="table">The type of the dataset</param>
    /// <param name="properties">The properties to search for</param>
    /// <typeparam name="TModel">The type of the provided data</typeparam>
    /// <returns>The filtered queryable</returns>
    public IQueryable<TModel> Search<TModel>(IQueryable<TModel> dataset, TableConfig table, IEnumerable<AdvancedSearchProperty> properties) where TModel : notnull;

}