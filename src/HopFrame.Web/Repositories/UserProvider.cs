using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Web.Admin;

namespace HopFrame.Web.Repositories;

internal sealed class UserProvider(IUserRepository repo) : ModelRepository<User> {
    public override async Task<IEnumerable<User>> ReadAll() {
        return await repo.GetUsers();
    }

    public override Task<User> Create(User model) {
        return repo.AddUser(model);
    }
    
    public override async Task<User> Update(User model) {
        await repo.UpdateUser(model);
        return model;
    }

    public override Task Delete(User model) {
        return repo.DeleteUser(model);
    }
}