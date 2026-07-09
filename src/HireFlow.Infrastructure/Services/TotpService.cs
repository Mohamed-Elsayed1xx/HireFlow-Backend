using System.Security.Cryptography;
using System.Text;
using HireFlow.Domain.Interfaces.Services;

namespace HireFlow.Infrastructure.Services;

public class TotpService : ITotpService
{
    private const int SecretByteLength = 20;
    private const int CodeDigits = 6;
    private const int StepSeconds = 30;
    private const string Issuer = "HireFlow";
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public string GenerateSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(SecretByteLength);
        return Base32Encode(bytes);
    }

    public string BuildAuthenticatorUri(string secret, string accountEmail)
    {
        var label = Uri.EscapeDataString($"{Issuer}:{accountEmail}");
        var issuer = Uri.EscapeDataString(Issuer);
        return $"otpauth://totp/{label}?secret={secret}&issuer={issuer}&algorithm=SHA1&digits={CodeDigits}&period={StepSeconds}";
    }

    public bool ValidateCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != CodeDigits || !code.All(char.IsDigit))
            return false;

        var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / StepSeconds;

        // Allow the previous and next 30-second window to tolerate clock drift
        // between the server and whatever device generated the code.
        for (var drift = -1; drift <= 1; drift++)
        {
            if (ComputeCode(secret, currentStep + drift) == code)
                return true;
        }

        return false;
    }

    private static string ComputeCode(string secret, long counter)
    {
        var keyBytes = Base32Decode(secret);
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);

        using var hmac = new HMACSHA1(keyBytes);
        var hash = hmac.ComputeHash(counterBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);

        var truncated = binaryCode % (int)Math.Pow(10, CodeDigits);
        return truncated.ToString().PadLeft(CodeDigits, '0');
    }

    private static string Base32Encode(byte[] data)
    {
        var result = new StringBuilder((data.Length * 8 + 4) / 5);
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var b in data)
        {
            buffer = (buffer << 8) | b;
            bitsLeft += 8;

            while (bitsLeft >= 5)
            {
                bitsLeft -= 5;
                result.Append(Base32Alphabet[(buffer >> bitsLeft) & 0x1F]);
            }
        }

        if (bitsLeft > 0)
            result.Append(Base32Alphabet[(buffer << (5 - bitsLeft)) & 0x1F]);

        return result.ToString();
    }

    private static byte[] Base32Decode(string base32)
    {
        var cleaned = base32.Trim().TrimEnd('=').ToUpperInvariant();
        var bytes = new List<byte>(cleaned.Length * 5 / 8);
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var c in cleaned)
        {
            var value = Base32Alphabet.IndexOf(c);
            if (value < 0)
                continue;

            buffer = (buffer << 5) | value;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                bitsLeft -= 8;
                bytes.Add((byte)((buffer >> bitsLeft) & 0xFF));
            }
        }

        return bytes.ToArray();
    }
}
