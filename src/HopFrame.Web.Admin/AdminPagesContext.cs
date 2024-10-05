using HopFrame.Web.Admin.Generators;

namespace HopFrame.Web.Admin;

public abstract class AdminPagesContext {

    public virtual void OnModelCreating(IAdminContextGenerator generator) {}

}