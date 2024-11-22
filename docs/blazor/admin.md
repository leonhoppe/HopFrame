# HopFrame Admin Pages
Admin pages can be defined through a `AdminContext` similar to how a `DbContext` is defined. They generate administration pages like [`/administration/users`](./pages.md)
simply by reading the structure of the provided model and optionally some additional configuration.

> **Fun fact:** The already existing pages `/administration/users` and `/administration/groups` are also generated using an internal `AdminContext`.

## Usage
1. Create a class that inherits the `AdminPagesContext` base class

    ```csharp
    public class AdminContext : AdminPagesContext {
        
    }
    ```
   
2. Add your admin pages as properties to the class

   ```csharp
   public class AdminContext : AdminPagesContext {
   
       public AdminPage<Address> Addresses { get; set; }
       
       public AdminPage<Employee> Employees { get; set; }
       
   }
   ```
   
   > **Hint:** you can specify the url of the admin page by adding the `AdminPageUrl` Attribute
   
3. **Optionally** you can further configure your pages in the `OnModelCreating` method

   ```csharp
   public class AdminContext : AdminPagesContext {
   
       public AdminPage<Address> Addresses { get; set; }
       public AdminPage<Employee> Employees { get; set; }
   
       public override void OnModelCreating(IAdminContextGenerator generator) {
           base.OnModelCreating(generator);
   
           generator.Page<Employee>()
               .Property(e => e.Address)
               .IsSelector();
   
           generator.Page<Address>()
               .Property(a => a.Employee)
               .Ignore();
   
           generator.Page<Address>()
               .Property(a => a.AddressId)
               .IsSelector<Employee>()
               .Parser<Employee>((model, e) => model.AddressId = e.EmployeeId);
   
           generator.Page<Employee>()
               .ConfigureRepository<EmployeeProvider>()
               .ListingProperty(e => e.Name);
   
           generator.Page<Address>()
               .ConfigureRepository<AddressProvider>()
               .ListingProperty(a => a.City);
       }
   }
   ```
4. **Optionally** you can also add some of the following attributes to your classes / properties to further configure the admin pages:\
   \
   Attributes for classes and properties:

   ```csharp
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
   public sealed class AdminNameAttribute(string name) : Attribute {
       public string Name { get; set; } = name;
   }
   ```

   Attributes for classes:

   ```csharp
   [AttributeUsage(AttributeTargets.Class)]
   public sealed class AdminButtonConfigAttribute(bool showCreateButton = true, bool showDeleteButton = true, bool showUpdateButton = true) : Attribute {
       public bool ShowCreateButton { get; set; } = showCreateButton;
       public bool ShowDeleteButton { get; set; } = showDeleteButton;
       public bool ShowUpdateButton { get; set; } = showUpdateButton;
   }
   ```

   ```csharp
   [AttributeUsage(AttributeTargets.Class)]
   public sealed class AdminPermissionsAttribute(string view = null, string create = null, string update = null, string delete = null) : Attribute {
       public AdminPagePermissions Permissions { get; set; } = new() {
           Create = create,
           Update = update,
           Delete = delete,
           View = view
       };
   }
   ```

   ```csharp
   [AttributeUsage(AttributeTargets.Class)]
   public sealed class AdminDescriptionAttribute(string description) : Attribute {
       public string Description { get; set; } = description;
   }
   ```
   
   Attributes for properties:

   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class AdminHideValueAttribute : Attribute;
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class AdminIgnoreAttribute(bool onlyForListing = false) : Attribute {
       public bool OnlyForListing { get; set; } = onlyForListing;
   }
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class AdminPrefixAttribute(string prefix) : Attribute {
       public string Prefix { get; set; } = prefix;
   }
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class AdminUneditableAttribute : Attribute;
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public class AdminUniqueAttribute : Attribute;
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class AdminUnsortableAttribute : Attribute;
   ```
   
   ```csharp
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class ListingPropertyAttribute : Attribute;
   ```
