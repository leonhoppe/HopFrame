using HopFrame.Web.Admin.Generators;

namespace HopFrame.Web.Admin;

public abstract class AdminPagesContext {

    public abstract void OnModelCreating(IAdminContextGenerator generator);

}