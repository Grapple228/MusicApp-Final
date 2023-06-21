using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Music.Identity.Service.Models;
using Music.Services.Identity.Jwt;

namespace Music.Identity.Service.Tools;

public static class PasswordProcessor
{
    /// <summary>
    ///     Генерирует зашифрованный пароль и соль
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="password">Шифруемый пароль</param>
    /// <returns>
    /// Password
    /// </returns>
    public static Password GenHashedPassword(Guid userId, string password)
    {
        var salt = JwtTokenHandler.GenRandomBytesArray(16);

        var hashedPassword = GetHashedPassword(password, salt);
        
        return new Password()
        {
            UserId = userId,
            Hash = hashedPassword,
            Salt = Convert.ToBase64String(salt)
        };
    }

    /// <summary>
    ///  Хеширует пароль и проверяет совпадение с хешированным паролем
    /// </summary>
    /// <param name="password">Нехешированный пароль</param>
    /// <param name="passwordData">Хешированный пароль</param>
    /// <returns>true - пароль верный, false - пароль неверный</returns>
    public static bool PasswordCompare(string password, Password passwordData)
    {
        var salt = Convert.FromBase64String(passwordData.Salt);

        var hashedPassword = GetHashedPassword(password, salt);

        return hashedPassword == passwordData.Hash;
    }

    private static string GetHashedPassword(string password, byte[] salt) =>
        Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));
}