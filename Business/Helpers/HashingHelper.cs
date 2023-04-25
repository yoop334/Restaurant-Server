

using System.Text;
using Business.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Model;

namespace Business.Helpers;

public class HashingHelper : IHashingHelper
{
    private readonly EncryptionSettings _encryptionSettings;

    public HashingHelper(IOptions<EncryptionSettings> encryptionSettings)
    {
        _encryptionSettings = encryptionSettings.Value;
    }

    public string HashPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            Encoding.ASCII.GetBytes(_encryptionSettings.Salt),
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8));
    }
}