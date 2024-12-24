# User Endpoints

## Used Models
- [User](../../models.md#user)
- [UserCreator](../../models.md#usercreator)
- [Permission](../../models.md#permission)

## API Endpoint: GetUsers

**Endpoint:** `GET /api/v1/users`

**Description:** Retrieves a list of users.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.read`

**Parameters:** None

**Response:**

- **200 OK:** Returns a list of users.
  ```json
  [
    {
      "Id": "guid",
      "Username": "string",
      "Email": "string",
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


## API Endpoint: GetUser

**Endpoint:** `GET /api/v1/users/{userId}`

**Description:** Retrieves a user by their ID.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.read`

**Parameters:**
- **userId:** `string` (Path) - The ID of the user to retrieve.

**Response:**

- **200 OK:** Returns the user details.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

- **400 Bad Request:** Invalid user ID format.

- **401 Unauthorized:** User is not authorized to access this endpoint.

- **404 Not Found:** User does not exist.


## API Endpoint: GetUserByUsername

**Endpoint:** `GET /api/v1/users/username/{username}`

**Description:** Retrieves a user by their username.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.read`

**Parameters:**
- **username:** `string` (Path) - The username of the user to retrieve.

**Response:**

- **200 OK:** Returns the user details.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

- **404 Not Found:** User does not exist.


## API Endpoint: GetUserByEmail

**Endpoint:** `GET /api/v1/users/email/{email}`

**Description:** Retrieves a user by their email address.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.read`

**Parameters:**
- **email:** `string` (Path) - The email address of the user to retrieve.

**Response:**

- **200 OK:** Returns the user details.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

- **404 Not Found:** User does not exist.


## API Endpoint: CreateUser

**Endpoint:** `POST /api/v1/users`

**Description:** Creates a new user.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.create`

**Parameters:**
- **UserCreator:** (Body) - The user creation details.
  ```json
  {
    "Username": "string",
    "Email": "string",
    "Password": "string",
    "Permissions": [
      "permission1",
      "permission2"
    ]
  }
  ```

**Response:**

- **200 OK:** Returns the created user.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

- **409 Conflict:** The user already exists.


## API Endpoint: UpdateUser

**Endpoint:** `PUT /api/v1/users/{userId}`

**Description:** Updates an existing user by their ID.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.update`

**Parameters:**
- **userId:** `string` (Path) - The ID of the user to update.
- **User:** (Body) - The user details to update.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

**Response:**

- **200 OK:** Returns the updated user.
  ```json
  {
    "Id": "guid",
    "Username": "string",
    "Email": "string",
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

- **400 Bad Request:** Invalid user ID format.

- **401 Unauthorized:** User is not authorized to access this endpoint.

- **404 Not Found:** User does not exist.

- **409 Conflict:** Cannot edit user with different user ID.


## API Endpoint: DeleteUser

**Endpoint:** `DELETE /api/v1/users/{userId}`

**Description:** Deletes a user by their ID.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.delete`

**Parameters:**
- **userId:** `string` (Path) - The ID of the user to delete.

**Response:**

- **200 OK:** User successfully deleted.

- **400 Bad Request:** Invalid user ID format.

- **401 Unauthorized:** User is not authorized to access this endpoint.

- **404 Not Found:** User does not exist.


## API Endpoint: ChangePassword

**Endpoint:** `PUT /api/v1/users/{userId}/password`

**Description:** Updates the password for a user by their ID.

**Authorization Required:** Yes

**Required Permission:** `hopframe.admin.users.update` (if the userId is not the id of the requesting user)

**Parameters:**
- **userId:** `string` (Path) - The ID of the user whose password is being changed.
- **UserPasswordChange:** (Body) - The password change details (note, if you change someone else's password the old password doesn't need to be correct).
  ```json
  {
    "oldPassword": "string",
    "newPassword": "string"
  }
  ```

**Response:**

- **200 OK:** Password successfully updated.

- **400 Bad Request:** Invalid user ID format.

- **401 Unauthorized:** User is not authorized to access this endpoint.

- **404 Not Found:** User does not exist.

- **409 Conflict:** Old password is incorrect.
