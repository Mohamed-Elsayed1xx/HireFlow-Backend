using HireFlow.Domain.Interfaces.Services;

namespace HireFlow.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}