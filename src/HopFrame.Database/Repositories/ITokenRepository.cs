using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface ITokenRepository {
    public Task<Token> GetToken(string content);
    public Task<Token> CreateToken(int type, User owner);
    public Task DeleteUserTokens(User owner);
}