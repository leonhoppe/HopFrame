# HopFrame Endpoints
HopFrame currently only supports endpoints for authentication out of the box.

> **Hint:** with the help of the [repositories](../repositories.md) you can very easily create missing endpoints for HopFrame components yourself.

## All currently supported endpoints

> **Hint:** you can use the build-in [swagger](https://swagger.io/) ui to explore and test all endpoints of your application __including__ HopFrame endpoints.

### SecurityController
Base endpoint: `/api/v1/authentication`\
**Important:** All primitive data types (including `string`) are return as a [`SingleValueResult`](./models.md#SingleValueResult)


| Method | Endpoint      | Payload                                                      | Returns               |
|--------|---------------|--------------------------------------------------------------|-----------------------|
| PUT    | /login        | [UserLogin](../models.md#UserLogin)                          | access token (string) |
| POST   | /register     | [UserRegister](../models.md#UserRegister)                    | access token (string) |
| GET    | /authenticate |                                                              | access token (string) |
| DELETE | /logout       |                                                              |                       |
| DELETE | /delete       | [UserPasswordValidation](./models.md#UserPasswordValidation) |                       |
