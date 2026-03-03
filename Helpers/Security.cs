using System;
using System.Security.Cryptography;
using System.Text;

namespace NexusGearMS.Helpers
{
    public static class Security
    {
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] ComputeHash(string password, byte[] salt)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
                byte[] combined = new byte[salt.Length + passwordBytes.Length];
                Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);
                return sha512.ComputeHash(combined);
            }
        }

        public static bool VerifyPassword(string password, byte[] salt, byte[] storedHash)
        {
            byte[] computedHash = ComputeHash(password, salt);
            return ConstantTimeCompare(computedHash, storedHash);
        }

        private static bool ConstantTimeCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        public static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return new byte[0];

            hex = hex.Replace("-", "").Replace(" ", "");

            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have even length");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
