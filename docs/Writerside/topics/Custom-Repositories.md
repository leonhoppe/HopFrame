# Custom Repositories

Custom repositories in HopFrame allow you to define and integrate custom logic for managing database entities. By implementing the `IHopFrameRepository<TModel, TKey>` interface, you can gain full control over how data is retrieved, modified, and managed. This feature is ideal for scenarios where the default behavior does not meet specific business requirements.

## IHopFrameRepository<TModel, TKey> Interface

The `IHopFrameRepository<TModel, TKey>` interface defines a contract for a repository that works with a specific model (`TModel`) and its primary key (`TKey`). The interface provides the following methods:

- **LoadPage**  
  Loads a paginated set of items.

  ```c#
  Task<IEnumerable<TModel>> LoadPage(int page, int perPage);
  ```

    - **Parameters:**
        - `page`: The page number to load.
        - `perPage`: The number of items per page.
    - **Returns:** A collection of items for the specified page.

- **Search**  
  Performs a search query on the repository.

  ```c#
  Task<SearchResult<TModel>> Search(string searchTerm, int page, int perPage);
  ```

    - **Parameters:**
        - `searchTerm`: The term to search for.
        - `page`: The page number to load.
        - `perPage`: The number of items per page.
    - **Returns:** A `SearchResult` containing matching items and the total number of pages.

- **GetTotalPageCount**  
  Retrieves the total number of pages based on the items per page.

  ```c#
  Task<int> GetTotalPageCount(int perPage);
  ```

    - **Parameters:**
        - `perPage`: The number of items per page.
    - **Returns:** The total number of pages.

- **CreateItem**  
  Adds a new item to the repository.

  ```c#
  Task CreateItem(TModel item);
  ```

    - **Parameters:**
        - `item`: The item to create.

- **EditItem**  
  Updates an existing item in the repository.

  ```c#
  Task EditItem(TModel item);
  ```

    - **Parameters:**
        - `item`: The item to update.

- **DeleteItem**  
  Removes an item from the repository.

  ```c#
  Task DeleteItem(TModel item);
  ```

    - **Parameters:**
        - `item`: The item to delete.

- **GetOne**  
  Retrieves a single item based on its primary key.

  ```c#
  Task<TModel?> GetOne(TKey key);
  ```

    - **Parameters:**
        - `key`: The primary key of the item to retrieve.
    - **Returns:** The item if found, or `null` if not.

## `SearchResult<TModel>` Struct

The `SearchResult<TModel>` struct is used to encapsulate the results of a search query.

- **Properties:**
    - `Items`: The items retrieved from the search query.
    - `PageCount`: The total number of pages based on the search results.

```c#
public readonly struct SearchResult<TModel>(IEnumerable<TModel> items, int pageCount) {
    public IEnumerable<TModel> Items { get; init; }
    public int PageCount { get; init; }
}
```

## Adding Custom Repositories

To add and configure a custom repository in HopFrame, use the `AddCustomRepository` methods. These methods allow you to specify a repository class (`TRepository`) implementing `IHopFrameRepository<TModel, TKey>` and define configurations for the associated table.

- **With Configurator**

  ```c#
  HopFrameConfigurator AddCustomRepository<TRepository, TModel, TKey>(
      Expression<Func<TModel, TKey>> keyExpression, 
      Action<TableConfigurator<TModel>> configurator
  )
      where TRepository : IHopFrameRepository<TModel, TKey>;
  ```

    - **Parameters:**
        - `keyExpression`: The key of the model.
        - `configurator`: Configures the table page.

- **Without Configurator**

  ```c#
  TableConfigurator<TModel> AddCustomRepository<TRepository, TModel, TKey>(
      Expression<Func<TModel, TKey>> keyExpression
  )
      where TRepository : IHopFrameRepository<TModel, TKey>;
  ```

    - **Parameters:**
        - `keyExpression`: The key of the model.
    - **Returns:** A `TableConfigurator` to configure the table.

By implementing custom repositories and using these methods, you can fully leverage the flexibility of HopFrame for your data management needs. Let me know if you'd like further elaboration!