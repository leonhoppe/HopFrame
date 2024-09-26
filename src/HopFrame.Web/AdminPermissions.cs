namespace HopFrame.Web;

[Obsolete("Use HopFrame.Security.AdminPermissions instead")]
public static class AdminPermissions {
    public const string IsAdmin = Security.AdminPermissions.IsAdmin;
    
    public const string ViewUsers = Security.AdminPermissions.ViewUsers;
    public const string EditUser = Security.AdminPermissions.EditUser;
    public const string DeleteUser = Security.AdminPermissions.DeleteUser;
    public const string AddUser = Security.AdminPermissions.AddUser;
    
    public const string ViewGroups = Security.AdminPermissions.ViewGroups;
    public const string EditGroup = Security.AdminPermissions.EditGroup;
    public const string DeleteGroup = Security.AdminPermissions.DeleteGroup;
    public const string AddGroup = Security.AdminPermissions.AddGroup;
}