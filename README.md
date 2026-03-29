# HopFrame

HopFrame is a lightweight extension library for existing .NET applications that adds **administrative Blazor pages** on top of your current data model. It connects to your configured databases and provides ready-to-use CRUD views so you can **browse, filter, create, update, and delete** data without writing custom admin UIs.

HopFrame is ideal for teams who want an instant admin interface without maintaining their own Blazor pages.

---

## Features

- **Plug-in admin UI**  
  Drop-in Blazor pages that integrate into an existing ASP.NET Core / Blazor application.

- **Database-driven views**  
  Reads metadata from your configured data sources and exposes entities as editable tables and forms.

- **Full CRUD support**  
  Create, read, update, and delete records directly from the browser.

- **Filtering & sorting**  
  Quickly navigate large datasets with built-in filtering and sorting capabilities.

- **Minimal boilerplate**  
  Focus on your domain logic—HopFrame handles the generic admin UI.

---

## Getting started

### 1. Install the package

```bash
dotnet add package HopFrame.Web
```

### 2. Register HopFrame in your application

In your `Program.cs` or `Startup.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register your DbContexts as usual
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure HopFrame
builder.Services.AddHopFrame(config => {
    config.AddDbContext<AppDbContext>();
});

var app = builder.Build();

// Map HopFrame admin endpoints
app.MapHopFrame();

// Or for existing blazor projects
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddHopFrame(); // Add HopFrame to the razor container

app.Run();
```

#### Additional configuration for Web APIs

Add the following code to your `.csproj` file:
```xml
<PropertyGroup>
    <RequiresAspNetWebAssets>true</RequiresAspNetWebAssets>
</PropertyGroup>
```

### 3. Navigate to the admin UI

After starting your application, open the configured admin route, for example:

```text
https://localhost:5001/admin
```

You'll see the HopFrame admin interface with your connected entities.

---

## Configuration

- **Entity discovery:**  
  HopFrame can be configured to scan your DbContexts and expose selected entities.
- **Access control:**  
  Protect the admin area using your existing authentication/authorization setup (e.g. policies, roles).
- **Customization:**  
  Override default pages, labels, or visibility for specific entities and properties.

### Configuration example

```csharp
builder.Services.AddHopFrame(config => {
    config.AddDbContext<DatabaseContext>();

    config.Table<User>(table => {
        table.SetDescription("The user dataset. It contains all information for the users of the application.");

        table.Property(u => u.Password)
            .Listable(false)
            .SetType(PropertyType.Password);

        table.AddProperty<string>("Username")
            .SetFormatter(u => $"{u.FirstName}.{u.LastName}".ToLower())
            .VisibleInEditor(false)
            .Listable(false);

        table.Property(u => u.Email)
            .SetValidator(input => {
                if (!input.Contains('.'))
                    return ["Email needs to contain a '.'"];

                return [];
            });

        table.SetPreferredProperty(u => u.Email);
    });

    config.Table<Post>(table => {
        table.SetDescription("The posts dataset. It contains all posts sent via the application.");
        table.SetEditClaim("edit.post");
    });

    config.Table<Typer>(table => {
        table.Property(t => t.LongText)
            .SetType(PropertyType.TextArea)
            .SetSizeRange(3..10);

        table.Property(t => t.Password)
            .SetType(PropertyType.Password);

        table.Property(t => t.PhoneNumber)
            .SetType(PropertyType.PhoneNumber);

        table.SetViewClaim("views.typer");
    });

    config.AddCustomPage(new() {
        Name = "Custom Page",
        Description = "This is a custom page",
        Icon = Icons.Material.Filled.House,
        Route = "/",
        OrderIndex = 2
    });

    config.SetCompanyName("Example");

    config.AddRepository<VirtualRepo, Dictionary<string, object?>>(table => {
        table.SetDisplayName("Addresses");
        table.SetRoute("addresses");
        
        table.AddProperty<int>("Id");
        table.AddProperty<string>("Country");
        table.AddProperty<string>("City");
        table.AddProperty<string>("Street");
        
        table.AddProperty<User>("Owner")
            .IsRelation(config.Table<User>());
    });
});
```

For more examples refer to the [debug projects](https://git.leon-hoppe.de/leon.hoppe/HopFrame/src/branch/dev/debug) in the repository.
