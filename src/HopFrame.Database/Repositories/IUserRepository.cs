using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface IUserRepository {
    Task<IList<User>> GetUsers();

    Task<User> GetUser(Guid userId);

    Task<User> GetUserByEmail(string email);

    Task<User> GetUserByUsername(string username);
    
    Task<User> AddUser(User user);

    /// <summary>
    /// IMPORTANT:<br/>
    /// This function does not add or remove any permissions to the user.
    /// For that please use <see cref="IPermissionRepository"/>
    /// </summary>
    Task UpdateUser(User user);

    Task DeleteUser(User user);

    Task<bool> CheckUserPassword(User user, string password);

    Task ChangePassword(User user, string password);
}