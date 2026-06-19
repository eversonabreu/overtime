namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface ICryptographyService
{
    string Encrypt(string value);
    string Decrypt(string value);
}