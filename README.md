# HopFrame

## Overview

Welcome to the **HopFrame**! This project aims to provide a comprehensive and modular framework for easy management of your database.
The framework is designed to be highly configurable, ensuring that developers either quickly add the framework for simple data editing or
configure it to their needs to implement it fully in their data management pipeline.

## Features

- **Dynamic Table Management**: Create, edit, and delete records dynamically with support for various data types including numeric, text, boolean, dates, and relational data.
- **Role-Based Access Control (RBAC)**: Implement fine-grained access control policies for viewing, creating, updating, and deleting records.
- **Modern Design**: A modern and user-friendly interface built with Fluent UI components, ensuring easy to use and pleasing administration pages.
- **Validation and Error Handling**: Comprehensive input validation and error handling to ensure data integrity and provide feedback to users.
- **Support for Complex Data Relationships**: Manage complex relationships between data entities with ease.

## Getting Started

### Installation

Install the nuget package using the CLI or the UI of your IDE:

```bash
dotnet add package HopFrame.Web
```

### Configuration

Configuring HopFrame is straightforward and flexible. You can easily define your contexts, tables, and their properties using the provided configurators.
Simply use your editors intelli-sense to find out what you can configure.

```csharp
builder.Services.AddHopFrame(options => {
    options.DisplayUserInfo(false);
    options.AddDbContext<DatabaseContext>(context => {
        context.Table<User>(table => {
            table.Property(u => u.Password)
                .DisplayValue(false);

            table.Property(u => u.FirstName)
                .List(false);

            table.Property(u => u.LastName)
                .List(false);

            table.Property(u => u.Id)
                .IsSortable(false)
                .SetOrderIndex(3);

            table.AddVirtualProperty("Name", (user, _) => $"{user.FirstName} {user.LastName}")
                .SetOrderIndex(2);

            table.SetDisplayName("Clients");
            table.SetDescription("This table is used for user data store and user authentication");

            table.SetViewPolicy("users.view");

            table.Property(u => u.Posts)
                .FormatEach<Post>((post, _) => post.Caption);
        });

        context.Table<Post>()
            .Property(p => p.Content)
            .IsTextArea(true)
            .Validator(input => {
                var errors = new List<string>();
                
                if (input is null)
                    errors.Add("Value cannot be null");
                
                if (input?.Length > 10)
                    errors.Add("Value can only be 10 characters long");

                return errors;
            });

        context.Table<Post>()
            .SetOrderIndex(-1);
    });
});
```

Then you need to map the frontend pages in your application:

```csharp
app.MapHopFrame();
```

### Usage

- Navigate to `/admin` to access the admin dashboard and start managing your tables.
- Use the side menu to switch between different tables.
- Utilize the built-in CRUD functionality to manage your data seamlessly.

## Contributing

Contributions are welcome! Feel free to open an issue or submit a pull request if you have suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
