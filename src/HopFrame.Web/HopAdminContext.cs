using System.Text.RegularExpressions;
using HopFrame.Database.Models;
using HopFrame.Security;
using HopFrame.Web.Admin;
using HopFrame.Web.Admin.Generators;
using HopFrame.Web.Admin.Models;
using HopFrame.Web.Repositories;

namespace HopFrame.Web;

public class HopAdminContext : AdminPagesContext {

    public AdminPage<User> Users { get; set; }
    public AdminPage<PermissionGroup> Groups { get; set; }
    
    public override void OnModelCreating(IAdminContextGenerator generator) {
        generator.Page<User>()
            .Description("On this page you can manage all user accounts.")
            .ConfigureRepository<UserProvider>()
            .ViewPermission(AdminPermissions.ViewUsers)
            .CreatePermission(AdminPermissions.AddUser)
            .UpdatePermission(AdminPermissions.EditUser)
            .DeletePermission(AdminPermissions.DeleteUser);

        generator.Page<User>().Property(u => u.Password)
            .DisplayInListing(false)
            .DisplayValueWhileEditing(false)
            .Validator(passwd => passwd.Length >= 8 ? null : "The password needs to be at least 8 characters long!");
        
        generator.Page<User>().Property(u => u.Email)
            .Validator(email => Regex.Match(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").Success ? null : "Invalid E-Mail address!")
            .Unique();

        generator.Page<User>().Property(u => u.Username)
            .Validator(uname => uname.Length >= 4 ? null : "The username needs to be at least 4 characters long!")
            .Unique();

        generator.Page<User>().Property(u => u.CreatedAt)
            .Editable(false);

        generator.Page<User>().Property(u => u.Permissions)
            .DisplayInListing(false)
            .DisplayPropertyForListType<Permission>(p => p.PermissionName)
            .ParserForListType<User, Permission>((user, perm) => new Permission {
                GrantedAt = DateTime.Now,
                PermissionName = perm,
                User = user
            });

        generator.Page<User>().Property(u => u.CreatedAt)
            .Generated();

        generator.Page<User>().Property(u => u.Id)
            .Generated();

        generator.Page<User>().Property(u => u.Tokens)
            .Ignore();


        generator.Page<PermissionGroup>()
            .Description("On this page you can view, create, edit and delete permission groups.")
            .ConfigureRepository<GroupProvider>()
            .ViewPermission(AdminPermissions.ViewGroups)
            .CreatePermission(AdminPermissions.AddGroup)
            .UpdatePermission(AdminPermissions.EditGroup)
            .DeletePermission(AdminPermissions.DeleteGroup);

        generator.Page<PermissionGroup>().Property(g => g.Name)
            .Prefix("group.");

        generator.Page<PermissionGroup>().Property(g => g.IsDefaultGroup)
            .DisplayName("Default Group")
            .Sortable(false);

        generator.Page<PermissionGroup>().Property(g => g.CreatedAt)
            .Generated();

        generator.Page<PermissionGroup>().Property(g => g.Permissions)
            .DisplayInListing(false)
            .DisplayPropertyForListType<Permission>(p => p.PermissionName)
            .ParserForListType<PermissionGroup, Permission>((group, perm) => new Permission {
                GrantedAt = DateTime.Now,
                PermissionName = perm,
                Group = group
            });
    }
}