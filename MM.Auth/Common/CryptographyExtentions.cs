using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MM.Auth.Common
{
    public static class CryptographyExtentions
    {
        /// <summary>
        /// Băm password với salt có sẵn.
        /// </summary>
        /// <param name="password">Password hashed</param>
        /// <param name="salt">Salt có sẵn</param>
        /// <returns>Password sau khi băm</returns>
        public static string HashHMACSHA256(this string password, string salt)
        {
            var realSalt = string.IsNullOrEmpty(salt) ? string.Empty : salt;
            return Hash(password, Encoding.ASCII.GetBytes(realSalt));
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        //Generate Verify Code
        public static string GenerateCode()
        {
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        //mã hoá password
        private static string Hash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public static bool VerifyPassword(string source, string password, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                return password.Equals(source);
            }
            var hashedPassword = password.HashHMACSHA256(salt);
            if (!hashedPassword.Equals(source))
            {
                return false;
            }
            return true;
        }

        public static string CreateToken(long Id, string Email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                    new Claim(ClaimTypes.Name, Email)
                }),
                Expires = DateTime.UtcNow.AddSeconds(AppSettings.ExpiredTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(SecurityToken);
            return Token;
        }
    }
}
