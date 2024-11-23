using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HopFrame.Database;

public static class EncryptionManager {

    /// <summary>
    /// Encrypts the given string with the specified hash method
    /// </summary>
    /// <param name="input">The raw string that should be hashed</param>
    /// <param name="salt">The "password" for the hash</param>
    /// <param name="method">The preferred hash method</param>
    /// <returns></returns>
    public static string Hash(string input, byte[] salt, KeyDerivationPrf method = KeyDerivationPrf.HMACSHA256) {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: input,
            salt: salt,
            prf: method,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        ));
    }
    
}