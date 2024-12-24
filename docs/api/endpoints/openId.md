# OpenID Endpoints

## Used Models
- [SingleValueResult](../../models.md#singlevalueresult)

## API Endpoint: RedirectToProvider

**Endpoint:** `GET /api/v1/openid/redirect`

**Description:** Redirects the user to the OpenID provider's authorization endpoint.

**Authorization Required:** No

**Parameters:**
- **redirectAfter** (query, optional): The URL to redirect to after authentication.
- **performRedirect** (query, optional): A flag to indicate if the user should be redirected (default is 1).

**Response:**
- **302 Found:** Redirects the user to the OpenID provider's authorization endpoint.
- **200 OK:** Returns the constructed authorization URI.
  ```json
  {
    "value": "string"
  }
  ```

## API Endpoint: Callback

**Endpoint:** `GET /api/v1/openid/callback`

**Description:** Handles the callback from the OpenID provider and exchanges the authorization code for tokens.

**Authorization Required:** No

**Parameters:**
- **code** (query, required): The authorization code received from the OpenID provider.
- **state** (query, optional): The state parameter to handle the redirect after authentication.

**Response:**
- **200 OK:** Returns the access token.
  ```json
  {
    "value": "string"
  }
  ```
- **400 Bad Request:** Authorization code is missing.
- **403 Forbidden:** Authorization code is not valid.

## API Endpoint: Refresh

**Endpoint:** `GET /api/v1/openid/refresh`

**Description:** Refreshes the access token using the refresh token.

**Authorization Required:** Yes

**Parameters:**
- None

**Response:**
- **200 OK:** Returns the refreshed access token.
  ```json
  {
    "value": "string"
  }
  ```
- **400 Bad Request:** Refresh token not provided.
- **409 Conflict**: Refresh token not valid.

## API Endpoint: Logout

**Endpoint:** `DELETE /api/v1/openid/logout`

**Description:** Logs out the user by deleting the authentication cookies.

**Authorization Required:** Yes

**Parameters:**
- None

**Response:**
- **200 OK:** User is logged out successfully.
