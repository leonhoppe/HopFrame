namespace HopFrame.Security;

public static class AdminPermissions {
    public const string IsAdmin = "hopframe.admin";
    
    public const string ViewUsers = "hopframe.admin.users.view";
    public const string EditUser = "hopframe.admin.users.edit";
    public const string DeleteUser = "hopframe.admin.users.delete";
    public const string AddUser = "hopframe.admin.users.add";
    
    public const string ViewGroups = "hopframe.admin.groups.view";
    public const string EditGroup = "hopframe.admin.groups.edit";
    public const string DeleteGroup = "hopframe.admin.groups.delete";
    public const string AddGroup = "hopframe.admin.groups.add";
}