using HopFrame.Security.Options;

namespace HopFrame.Security.Authorization;

public class AdminPermissionOptions : OptionsFromConfiguration {
    public override string Position { get; } = "HopFrame:Permissions";
    
    public string Dashboard { get; set; } = "hopframe.admin";

    public CrudPermission Users { get; set; } = new() {
        Read = "hopframe.admin.users.read",
        Update = "hopframe.admin.users.update",
        Delete = "hopframe.admin.users.delete",
        Create = "hopframe.admin.users.create"
    };
    
    public CrudPermission Groups { get; set; } = new() {
        Read = "hopframe.admin.groups.read",
        Update = "hopframe.admin.groups.update",
        Delete = "hopframe.admin.groups.delete",
        Create = "hopframe.admin.groups.create"
    };
    
    public class CrudPermission {
        public string Create { get; set; }
        public string Read { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
    }
}