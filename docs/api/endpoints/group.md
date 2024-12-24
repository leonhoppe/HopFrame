# Group Endpoints

## Used Models
- [Group](../../models.md#permissiongroup)

## API Endpoint: GetGroups

**Endpoint:** `GET /api/v1/groups`

**Description:** Retrieves a list of all groups.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.read`

**Parameters:** None

**Response:**

- **200 OK:** Returns a list of groups.
  ```json
  [
    {
      "Name": "string",
      "IsDefaultGroup": "boolean",
      "Description": "string",
      "CreatedAt": "2023-12-23T00:00:00Z",
      "Permissions": [
        {
          "Id": 1,
          "PermissionName": "string",
          "GrantedAt": "2023-12-23T00:00:00Z"
        }
      ]
    }
  ]
  ```
- **401 Unauthorized:** User is not authorized to access this endpoint.

## API Endpoint: GetDefaultGroups

**Endpoint:** `GET /api/v1/groups/default`

**Description:** Retrieves a list of default groups.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.read`

**Parameters:** None

**Response:**

- **200 OK:** Returns a list of default groups.
  ```json
  [
    {
      "Name": "string",
      "IsDefaultGroup": "boolean",
      "Description": "string",
      "CreatedAt": "2023-12-23T00:00:00Z",
      "Permissions": [
        {
          "Id": 1,
          "PermissionName": "string",
          "GrantedAt": "2023-12-23T00:00:00Z"
        }
      ]
    }
  ]
  ```
- **401 Unauthorized:** User is not authorized to access this endpoint.

## API Endpoint: GetUserGroups

**Endpoint:** `GET /api/v1/groups/user/{userId}`

**Description:** Retrieves a list of groups for a specific user.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.read`

**Parameters:**

- **userId:** `string` (path) - The ID of the user.

**Response:**

- **200 OK:** Returns a list of groups for the user.
  ```json
  [
    {
      "Name": "string",
      "IsDefaultGroup": "boolean",
      "Description": "string",
      "CreatedAt": "2023-12-23T00:00:00Z",
      "Permissions": [
        {
          "Id": 1,
          "PermissionName": "string",
          "GrantedAt": "2023-12-23T00:00:00Z"
        }
      ]
    }
  ]
  ```
- **400 Bad Request:** Invalid user ID.
- **401 Unauthorized:** User is not authorized to access this endpoint.
- **404 Not Found:** User does not exist.

## API Endpoint: GetGroup

**Endpoint:** `GET /api/v1/groups/{name}`

**Description:** Retrieves details of a specific group.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.read`

**Parameters:**

- **name:** `string` (path) - The name of the group.

**Response:**

- **200 OK:** Returns the details of the group.
  ```json
  {
    "Name": "string",
    "IsDefaultGroup": "boolean",
    "Description": "string",
    "CreatedAt": "2023-12-23T00:00:00Z",
    "Permissions": [
      {
        "Id": 1,
        "PermissionName": "string",
        "GrantedAt": "2023-12-23T00:00:00Z"
      }
    ]
  }
  ```
- **401 Unauthorized:** User is not authorized to access this endpoint.
- **404 Not Found:** Group does not exist.

## API Endpoint: CreateGroup

**Endpoint:** `POST /api/v1/groups`

**Description:** Creates a new group.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.create`

**Parameters:**

- **group:** `PermissionGroup` (body) - The group to be created.

**Response:**

- **200 OK:** Returns the created group.
  ```json
  {
    "Name": "string",
    "IsDefaultGroup": "boolean",
    "Description": "string",
    "CreatedAt": "2023-12-23T00:00:00Z",
    "Permissions": [
      {
        "Id": 1,
        "PermissionName": "string",
        "GrantedAt": "2023-12-23T00:00:00Z"
      }
    ]
  }
  ```
- **400 Bad Request:** Provide a group.
- **400 Bad Request:** Group names must start with 'group.'.
- **401 Unauthorized:** User is not authorized to access this endpoint.
- **409 Conflict:** Group already exists.

## API Endpoint: UpdateGroup

**Endpoint:** `PUT /api/v1/groups`

**Description:** Updates an existing group.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.update`

**Parameters:**

- **group:** `PermissionGroup` (body) - The group to be updated.

**Response:**

- **200 OK:** Returns the updated group.
  ```json
  {
    "Name": "string",
    "IsDefaultGroup": "boolean",
    "Description": "string",
    "CreatedAt": "2023-12-23T00:00:00Z",
    "Permissions": [
      {
        "Id": 1,
        "PermissionName": "string",
        "GrantedAt": "2023-12-23T00:00:00Z"
      }
    ]
  }
  ```
- **401 Unauthorized:** User is not authorized to access this endpoint.
- **404 Not Found:** Group does not exist.

## API Endpoint: DeleteGroup

**Endpoint:** `DELETE /api/v1/groups/{name}`

**Description:** Deletes a specific group.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.groups.delete`

**Parameters:**

- **name:** `string` (path) - The name of the group to be deleted.

**Response:**

- **200 OK:** Group deleted successfully.
- **401 Unauthorized:** User is not authorized to access this endpoint.
- **404 Not Found:** Group does not exist.
