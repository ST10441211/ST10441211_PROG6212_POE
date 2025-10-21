using System;
using System.Security.Cryptography;

namespace ST10441211_PROG6212_POE.Services
{
    public static class PasswordHasher
    {
        // PBKDF2
        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(32);
                var result = new byte[49]; // 16 salt + 32 hash + 1 version
                result[0] = 0; // version
                Buffer.BlockCopy(salt, 0, result, 1, 16);
                Buffer.BlockCopy(hash, 0, result, 17, 32);
                return Convert.ToBase64String(result);
            }
        }

        public static bool Verify(string password, string stored)
        {
            var bytes = Convert.FromBase64String(stored);
            if (bytes[0] != 0) return false; // version mismatch
            var salt = new byte[16];
            Buffer.BlockCopy(bytes, 1, salt, 0, 16);
            var storedHash = new byte[32];
            Buffer.BlockCopy(bytes, 17, storedHash, 0, 32);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(32);
                return CryptographicOperations.FixedTimeEquals(hash, storedHash);
            }
        }
    }
}