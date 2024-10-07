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
            .DisplayValueWhileEditing(false);

        generator.Page<User>().Property(u => u.CreatedAt)
            .Editable(false);

        generator.Page<User>().Property(u => u.Permissions)
            .DisplayInListing(false);

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
            .Sortable(false);

        generator.Page<PermissionGroup>().Property(g => g.CreatedAt)
            .Editable(false);

        generator.Page<PermissionGroup>().Property(g => g.Permissions)
            .DisplayInListing(false);
    }
}