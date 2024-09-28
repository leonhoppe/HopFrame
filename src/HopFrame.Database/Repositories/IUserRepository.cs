using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface IUserRepository {
    Task<IList<User>> GetUsers();

    Task<User> GetUser(Guid userId);

    Task<User> GetUserByEmail(string email);

    Task<User> GetUserByUsername(string username);
    
    Task<User> AddUser(User user);
    
    Task UpdateUser(User user);

    Task DeleteUser(User user);

    Task<bool> CheckUserPassword(User user, string password);

    Task ChangePassword(User user, string password);
}