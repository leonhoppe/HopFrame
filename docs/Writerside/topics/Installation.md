# Installation

Install the nuget package using the CLI or the UI of your IDE:

```bash
dotnet add package HopFrame.Web
```

## Minimal configuration

Configuring HopFrame is straightforward and flexible. You can easily define your contexts, tables, and their properties using the provided configurators.
Simply use your editors intelli-sense to find out what you can configure.


```c#
builder.Services.AddHopFrame(options => {
    options.AddDbContext<DatabaseContext>();
});
```

Then you need to map the frontend pages in your application:

```c#
app.MapHopFrame();
```

## Usage

- Navigate to `/admin` to access the admin dashboard and start managing your tables.
- Use the side menu to switch between different tables.
- Utilize the built-in CRUD functionality to manage your data seamlessly.
