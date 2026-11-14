using Microsoft.AspNetCore.Identity;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Api.Services;

public sealed class PasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password cannot be empty.",
                nameof(password));

        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(
        User user,
        string password,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var result = _hasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
