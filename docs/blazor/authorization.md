# HopFrame Authentication

With the provided HopFrame services, you can secure your blazor pages so that only logged-in users or users with the right permissions can access the page.

## Usage
You can secure your Blazor pages by using the `AuthorizedView` component.
Everything placed inside this component will only be displayed if the authorization was successful.
You can also redirect the user if the authorization fails by specifying a `RedirectIfUnauthorized` url.

```html
<!-- You can either specify one 'Permission', multiple 'Permissions' or none if the user only needs to be logged-in -->
<AuthorizedView Permission="test.permission">
    <p>This paragraph is only visible if the user is logged-in and has the required permission</p>
</AuthorizedView>
```

```html
<!-- This component will redirect the user to the login page if the user is unauthorized -->
<AuthorizedView RedirectIfUnauthorized="/login" />
```
