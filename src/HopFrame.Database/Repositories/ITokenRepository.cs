using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface ITokenRepository {
    Task<Token> GetToken(string content);
    Task<Token> CreateToken(int type, User owner);
    Task DeleteUserTokens(User owner);
}