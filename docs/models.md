# Models for HopFrame

This page shows all models that HopFrame uses.


## Base Models
These are the models used by the various database services.

```plantuml
set namespaceSeparator none

namespace HopFrame.Database {
    class User {
        +Id: Guid
        +Username: string
        +Email: string
        +CreatedAt: DateTime
        +Permissions: IList<Permission>
    }
    
    class Permission {
        +Id: long
        +PermissionName: string
        +Owner: Guid
        +GrantedAt: DateTime
    }
    
    class PermissionGroup {
        +Name: string
        +IsDefaultGroup: bool
        +Description: string
        +CreatedAt: DateTime
        +Permissions: IList<Permission>
    }
    
    interface IPermissionOwner {}
}

IPermissionOwner <|-- User
IPermissionOwner <|-- PermissionGroup

User .. Permission
PermissionGroup .. Permission
```


## API Models
These are the models used by the REST API and the Blazor API.

```plantuml
namespace HopFrame.Security {
    class UserLogin {
        +Email: string
        +Password: string
    }
    
    class UserRegister {
        +Username: string
        +Email: string
        +Password: string
    }
}

namespace HopFrame.Api {
    class SingleValueResult<TValue> {
        +Value: TValue
    }
    
    class UserPasswordValidation {
        +Password: string
    }
}
```


## Database Models
These are the models that correspond to the scheme in the Database

```plantuml
set namespaceSeparator none

namespace HopFrame.Database {
    class UserEntry {
        +Id: string
        +Username: string
        +Email: string
        +Password: string
        +CreatedAt: DateTime
    }
    
    class TokenEntry {
        +Type: int
        +Token: string
        +UserId: string
        +CreatedAt: DateTime
    }
    
    class PermissionEntry {
        +RecordId: long
        +PermissionText: string
        +UserId: string
        +GrantedAt: DateTime
    }
    
    class GroupEntry {
        +Name: string
        +Default: bool
        +Description: string
        +CreatedAt: DateTime
    }
}

UserEntry *-- TokenEntry
UserEntry *-- PermissionEntry
```
