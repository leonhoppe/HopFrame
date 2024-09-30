using System.ComponentModel;
using HopFrame.Database.Models;
using HopFrame.Web.Admin;
using HopFrame.Web.Admin.Generators;
using HopFrame.Web.Admin.Models;

namespace RestApiTest;

public class AdminContext : AdminPagesContext {

    public AdminPage<User> Users { get; set; }
    public AdminPage<PermissionGroup> Groups { get; set; }
    
    public override void OnModelCreating(IAdminContextGenerator generator) {

        /*generator.Page<User>()
            .Title("Users")
            .Description("On this page you can manage all user accounts.")
            .UpdatePermission("update")
            .ViewPermission("view")
            .DeletePermission("delete")
            .CreatePermission("create")
            .DefaultSort(u => u.Id, ListSortDirection.Descending);*/

    }
}