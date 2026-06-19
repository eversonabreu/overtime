using Evertech.Overtime.Domain.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Evertech.Overtime.Domain.Services.Implementations;

[ExcludeFromCodeCoverage]
internal sealed class CryptographyService(IConfiguration configuration) : ICryptographyService
{
    public string Encrypt(string value)
    {
        using var provider = TripleDES.Create();
        var bytes = Encoding.UTF8.GetBytes(configuration.GetSection("CryptographyKey").Value);
        provider.Key = MD5.HashData(bytes);
        provider.Mode = CipherMode.ECB;
        provider.Padding = PaddingMode.PKCS7;
        var encryptor = provider.CreateEncryptor();
        var buffer = Encoding.UTF8.GetBytes(value);
        var encryptorBytes = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        string result = Convert.ToBase64String(encryptorBytes);
        return result;
    }

    public string Decrypt(string value)
    {
        using var provider = TripleDES.Create();
        var bytes = Encoding.UTF8.GetBytes(configuration.GetSection("CryptographyKey").Value);
        provider.Key = MD5.HashData(bytes);
        provider.Mode = CipherMode.ECB;
        provider.Padding = PaddingMode.PKCS7;
        var decryptor = provider.CreateDecryptor();
        var buffer = Convert.FromBase64String(value);
        var decriptorBytes = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        string result = Encoding.UTF8.GetString(decriptorBytes);
        return result;
    }
}