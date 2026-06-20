using System.Security.Cryptography;
using System.Text;

namespace Evertech.Overtime.Domain.Helpers;

public static class PasswordGeneratorHelper
{
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string Symbols = "@$!%*?&";
    private const string AllChars = Lowercase + Uppercase + Digits + Symbols;

    public static string GenerateComplex(int length = 14)
    {
        if (length < 14) length = 14;
        if (length > 20) length = 20;

        var password = new StringBuilder();
        password.Append(Lowercase[RandomNumberGenerator.GetInt32(Lowercase.Length)]);
        password.Append(Uppercase[RandomNumberGenerator.GetInt32(Uppercase.Length)]);
        password.Append(Digits[RandomNumberGenerator.GetInt32(Digits.Length)]);
        password.Append(Symbols[RandomNumberGenerator.GetInt32(Symbols.Length)]);

        for (int index = password.Length; index < length; index++)
        {
            password.Append(AllChars[RandomNumberGenerator.GetInt32(AllChars.Length)]);
        }

        return ShuffleString(password.ToString());
    }

    private static string ShuffleString(string str)
    {
        char[] array = str.ToCharArray();
        int length = array.Length;
        while (length > 1)
        {
            int index = RandomNumberGenerator.GetInt32(length--);
            (array[index], array[length]) = (array[length], array[index]);
        }
        return new string(array);
    }
}