# HopFrame Pages
By default, the HopFrame provides some blazor pages for managing user accounts and permissions

## All currently supported blazor pages

| Page            | Endpoint               | Permission                 | Usage                                                                                                  |
|-----------------|------------------------|----------------------------|--------------------------------------------------------------------------------------------------------|
| Admin Dashboard | /administration        | hopframe.admin             | This page provides an overview to all admin pages built-in and created by [AdminContexts](./admin.md). |
| Admin Login     | /administration/login  |                            | This page is a simple login screen so no login screen needs to be created to access the admin pages.   |
| User Dashboard  | /administration/users  | hopframe.admin.users.view  | This page serves as a management site for all users and their permissions.                             |
| Group Dashboard | /administration/groups | hopframe.admin.groups.view | This page serves as a management site for all groups and their permissions.                            |

> **Hint:** All pages created by [AdminContexts](./admin.md) are also under the `/administration/` location. This can unfortunately __not__ be changed at the moment.

