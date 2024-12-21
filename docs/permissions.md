# HopFrame Permissions

Permissions in the HopFrame are simple and effective to use.
As discussed in the [repositories](./repositories.md) documentation, you can manage user / group permissions
via the `IPermissionRepository` service.

## How do permissions work in the HopFrame

Permissions are defined using the . (dot) syntax. This enables you to nest permissions in namespaces.
You can also give a user or a group the permission to every permission in a namespace by using the * (star) syntax.

| Permission           | Example                       | Description                                           |
|----------------------|-------------------------------|-------------------------------------------------------|
| `*`                  | `*`                           | all permissions                                       |
| `[namespace].[name]` | `hopframe.admin.users.create` | single permission                                     |
| `[namespace].*`      | `hopframe.admin.*`            | all permissions in that namespace (works recursively) |

### Reserved namespaces

| Namespace | Example       | Description                              |
|-----------|---------------|------------------------------------------|
| `group`   | `group.admin` | The user needs to be in a specific group |

### Permission Groups

You can manage them through the `IGroupRepository` as described in the [repositories](./repositories.md) documentation.
You add permissions just like you would to a user with the `IPermissionRepository`.
You can assign a user to a group by assigning the group permission to the user:
```csharp
permissionRepository.AddPermission(user, "group.admin");
```

## Predefined Permissions

| Permission                     | Description                   |
|--------------------------------|-------------------------------|
| `hopframe.admin`               | Access to the admin dashboard |
| `hopframe.admin.users.read`    | View all users                |
| `hopframe.admin.users.update`  | Edit a user                   |
| `hopframe.admin.users.delete`  | Delete a user                 |
| `hopframe.admin.users.create`  | Add a group                   |
| `hopframe.admin.groups.read`   | View all groups               |
| `hopframe.admin.groups.update` | Edit a group                  |
| `hopframe.admin.groups.delete` | Delete a group                |
| `hopframe.admin.groups.create` | Add a group                   |

### Configuring HopFrame permissions

You can also configure the predefined permissions using the `appsettings.json` or environment variables
by configuring your configuration to load these.
>**Hint**: Configuring your application to use environment variables works by simply adding
> `builder.Configuration.AddEnvironmentVariables();` to your startup configuration before you add the
> custom configurations / HopFrame services.

You can specify `Dashboard` for the dashboard permission and for `Users` and `Groups` you can specify
`Create`, `Read`, `Update` and `Delete` permissions.

#### Configuration example
```json
  "HopFrame": {
    "Permissions": {
      "Dashboard": "myapp.dashboard.view",
      "Users": {
        "Read": "myapp.read.users"
      },
      "Groups": {
        "Create": "myapp.create.groups",
        "Update": "myapp.update.groups"
      }
    }
  }
```

#### Environment variables example
```dotenv
HOPFRAME__PERMISSIONS__DASHBOARD="myapp.dashboard.view"
HOPFRAME__PERMISSIONS__USERS__READ="myapp.read.users"
HOPFRAME__PERMISSIONS__GROUPS__CREATE="myapp.create.groups"
HOPFRAME__PERMISSIONS__GROUPS__UPDATE="myapp.update.groups"
```
