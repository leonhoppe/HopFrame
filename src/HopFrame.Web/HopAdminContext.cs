using System.Text.RegularExpressions;
using HopFrame.Database.Models;
using HopFrame.Security;
using HopFrame.Security.Authorization;
using HopFrame.Web.Admin;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Generators;
using HopFrame.Web.Admin.Models;
using HopFrame.Web.Provider;
using Microsoft.Extensions.Options;

namespace HopFrame.Web;

internal class HopAdminContext(IOptions<AdminPermissionOptions> options) : AdminPagesContext {

    [AdminPageUrl("users")]
    public AdminPage<User> Users { get; set; }
    
    [AdminPageUrl("groups")]
    public AdminPage<PermissionGroup> Groups { get; set; }
    
    public override void OnModelCreating(IAdminContextGenerator generator) {
        generator.Page<User>()
            .Description("On this page you can manage all user accounts.")
            .ConfigureProvider<UserProvider>()
            .ReadPermission(options.Value.Users.Read)
            .CreatePermission(options.Value.Users.Create)
            .UpdatePermission(options.Value.Users.Update)
            .DeletePermission(options.Value.Users.Delete);

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
            .DisplayProperty<Permission>(p => p.PermissionName)
            .Parser<string, Permission>((user, perm) => new Permission {
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
            .ConfigureProvider<GroupProvider>()
            .ReadPermission(options.Value.Groups.Read)
            .CreatePermission(options.Value.Groups.Create)
            .UpdatePermission(options.Value.Groups.Update)
            .DeletePermission(options.Value.Groups.Delete)
            .ListingProperty(g => g.Name);

        generator.Page<PermissionGroup>().Property(g => g.Name)
            .Prefix("group.");

        generator.Page<PermissionGroup>().Property(g => g.IsDefaultGroup)
            .DisplayName("Default Group")
            .Sortable(false);

        generator.Page<PermissionGroup>().Property(g => g.CreatedAt)
            .Generated();

        generator.Page<PermissionGroup>().Property(g => g.Permissions)
            .DisplayInListing(false)
            .DisplayProperty<Permission>(p => p.PermissionName)
            .Parser<string, Permission>((group, perm) => new Permission {
                GrantedAt = DateTime.Now,
                PermissionName = perm,
                Group = group
            });
    }
}