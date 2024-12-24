# Auth Endpoints

## Used Models
- [UserLogin](../../models.md#userlogin)
- [UserRegister](../../models.md#userregister)
- [SingleValueResult](../../models.md#singlevalueresult)
- [UserPasswordValidation](../../models.md#userpasswordvalidation)

## API Endpoint: Login

**Endpoint:** `PUT /api/v1/auth/login`

**Description:** Authenticates a user and provides access and refresh tokens.

**Authorization Required:** No

**Parameters:**
- **UserLogin** (required): The login credentials of the user.
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```

**Response:**
- **200 OK:** Returns the access token.
  ```json
  {
    "value": "string"
  }
  ```
- **400 Bad Request:** HopFrame authentication scheme is disabled.
- **404 Not Found:** The provided email address was not found.
- **403 Forbidden:** The provided password is not correct.

## API Endpoint: Register

**Endpoint:** `POST /api/v1/auth/register`

**Description:** Registers a new user and provides access and refresh tokens.

**Authorization Required:** No

**Parameters:**
- **UserRegister** (required): The registration details of the user.
  ```json
  {
    "username": "string",
    "email": "string",
    "password": "string"
  }
  ```

**Response:**
- **200 OK:** Returns the access token.
  ```json
  {
    "value": "string"
  }
  ```
- **400 Bad Request:** HopFrame authentication scheme is disabled or the password is too short.
- **409 Conflict:** Username or email is already registered.

## API Endpoint: Authenticate

**Endpoint:** `GET /api/v1/auth/authenticate`

**Description:** Authenticates the user using the refresh token and provides a new access token.

**Authorization Required:** Yes

**Parameters:**
- None

**Response:**
- **200 OK:** Returns the access token.
  ```json
  {
    "value": "string"
  }
  ```
- **400 Bad Request:** HopFrame authentication scheme is disabled or refresh token not provided.
- **404 Not Found:** The refresh token is not valid.
- **403 Forbidden:** The refresh token is expired.
- **409 Conflict:** The provided token is not a refresh token.

## API Endpoint: Logout

**Endpoint:** `DELETE /api/v1/auth/logout`

**Description:** Logs out the user and deletes the access and refresh tokens.

**Authorization Required:** Yes

**Parameters:**
- None

**Response:**
- **200 OK:** User is logged out successfully.

## API Endpoint: Delete

**Endpoint:** `DELETE /api/v1/auth/delete`

**Description:** Deletes the user account.

**Authorization Required:** Yes

**Parameters:**
- **UserPasswordValidation** (required): The password validation for the user.
  ```json
  {
    "password": "string"
  }
  ```

**Response:**
- **200 OK:** User account is deleted successfully.
- **403 Forbidden:** The provided password is not correct.
