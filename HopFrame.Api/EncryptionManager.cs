using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HopFrame.Api;

public static class EncryptionManager {

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