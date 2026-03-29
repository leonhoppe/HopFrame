using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;

namespace HopFrame.Core.Services;

/// A service used to sort a dataset by a specific property
public interface ISortService {

    /// <summary>
    /// Sorts the provided dataset by the given property
    /// </summary>
    /// <param name="dataset">The dataset that should be sorted</param>
    /// <param name="sorting">The sorting information</param>
    /// <param name="table">The type of the dataset</param>
    /// <typeparam name="TModel">The type of the provided data</typeparam>
    /// <returns>The sorted queryable</returns>
    public IQueryable<TModel> Sort<TModel>(IQueryable<TModel> dataset, Sorting sorting, TableConfig table) where TModel : class;

}